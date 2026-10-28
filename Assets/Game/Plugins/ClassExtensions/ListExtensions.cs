using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ListExtensions
{
	private static System.Random random = new();
	
	public static bool Solid<T>(this List<T> list)
	{
		return list is { Count: > 0 };
	}
	
	public static List<T> TakeAndRemove<T>(this List<T> list, int count)
	{
		if (list  == null) throw new ArgumentNullException(nameof(list));
		if (count <= 0) return new List<T>();
		
		var actualCount = Math.Min(count, list.Count);
		var result      = list.GetRange(0, actualCount);
		list.RemoveRange(0, actualCount);
		return result;
	}
	
	public static T RandomElement<T>(this List<T> list)
	{
		if (list == null)
			throw new InvalidOperationException("Cannot select a random element from an null list.");
		if (list.Count == 0)
			throw new InvalidOperationException("Cannot select a random element from an empty list.");
		
		var index = random.Next(list.Count);
		return list[index];
	}
	
	public static List<T> RandomElements<T>(this List<T> list, int count)
	{
		if (list == null || list.Count == 0 || count <= 0)
			throw new InvalidOperationException("Invalid list or count.");
		
		count = Mathf.Min(count, list.Count);
		List<T> shuffledList = new List<T>(list);
		shuffledList.Shuffle();
		
		return shuffledList.GetRange(0, count);
	}
	
	public static void Shuffle<T>(this List<T> list)
	{
		for (int i = list.Count - 1; i > 0; i--)
		{
			int j = random.Next(i + 1);
			(list[i], list[j]) = (list[j], list[i]); // Swap
		}
	}
	
	public static void RemoveN<T>(this List<T> list, T value, int n)
	{
		if (list == null || n <= 0) return;
		
		int count = 0;
		for (int i = 0; i < list.Count && count < n; i++)
		{
			if (EqualityComparer<T>.Default.Equals(list[i], value))
			{
				list.RemoveAt(i);
				i--; // Lùi lại 1 index do list đã bị thay đổi
				count++;
			}
		}
	}
	
	public static List<T> Distribute<T>(this List<T> list, int count, int multipleOf)
	{
		if (list == null || list.Count < count * multipleOf)
			throw new ArgumentException("List không đủ phần tử để phân phối.");
		
		var result          = new List<T>();
		int remainingGroups = count;
		
		while (remainingGroups > 0 && list.Count >= multipleOf)
		{
			// Chọn random 1 phần tử trong list
			var candidate = list[UnityEngine.Random.Range(0, list.Count)];
			
			// Tìm các phần tử giống candidate
			var group = list.Where(x => EqualityComparer<T>.Default.Equals(x, candidate)).Take(multipleOf).ToList();
			
			// Nếu đủ thì thêm vào result và xóa khỏi list
			if (group.Count == multipleOf)
			{
				result.AddRange(group);
				foreach (var item in group)
					list.Remove(item);
				
				remainingGroups--;
			}
			else
			{
				// Không đủ phần tử giống nhau => thử lại với phần tử khác
				continue;
			}
		}
		
		// Nếu vẫn chưa đủ nhóm, thử các phần tử còn lại (bất kể loại nào)
		while (remainingGroups > 0 && list.Count >= multipleOf)
		{
			var group = list.Take(multipleOf).ToList();
			result.AddRange(group);
			foreach (var item in group)
				list.Remove(item);
			
			remainingGroups--;
		}
		
		if (remainingGroups > 0)
			Debug.LogWarning($"Không đủ phần tử phù hợp. Đã phân phối được {count - remainingGroups} / {count} nhóm.");
		
		return result;
	}
}