using System;
using UnityEngine;

public class ScreenInWorld : MonoBehaviour
{
    [SerializeField] private Camera camera;
    [SerializeField] private bool isInstanceOfSingleton;

    [Header("Min Screen Size")]
    public bool applyMinScreenSizeInUpdate = false;
    public bool useMinWidth;
    public float minWidth = 7.2f;
    
    public bool useMinHeight;
    public float minHeight = 12.8f;


    
    private static ScreenInWorld instance;

    public static ScreenInWorld Instance => instance;

    private static float realScreenWidth;
    private static float realScreenHeight;
    
    

    #region Some pos

    public float Width
    {
        get => (camera.orthographicSize * realScreenWidth / realScreenHeight) * 2;
        set => camera.orthographicSize = (value / (realScreenWidth / realScreenHeight)) / 2f;
    }

    public float Height
    {
        get => camera.orthographicSize * 2;
        set => camera.orthographicSize = value / 2f;
    }

    public float HalfWidth
    {
        get => camera.orthographicSize * realScreenWidth / realScreenHeight;
        set => camera.orthographicSize = value / (realScreenWidth / realScreenHeight);
    }

    public float HalfHeight
    {
        get => camera.orthographicSize;
        set => camera.orthographicSize = value;
    }

    public float MaxPosY => transform.position.y + HalfHeight;
    public float MinPosY => transform.position.y - HalfHeight;
    
    public float MaxPosX => transform.position.x + HalfWidth;
    public float MinPosX => transform.position.x - HalfWidth;
    
    public Vector2 TopLeft => new Vector2(MinPosX, MaxPosY);
    public Vector2 TopMiddle => new Vector2(transform.position.x, MaxPosY);
    public Vector2 TopRight => new Vector2(MaxPosX, MaxPosY);
    
    public Vector2 BottomLeft => new Vector2(MinPosX, MinPosY);
    public Vector2 BottomMiddle => new Vector2(transform.position.x, MinPosY);
    public Vector2 BottomRight => new Vector2(MaxPosX, MinPosY);
    
    public Vector2 MiddleLeft => new Vector2(MinPosX, transform.position.y);
    public Vector2 MiddleRight => new Vector2(MaxPosX, transform.position.y);

    public Vector2 Center => transform.position;
    
    public Vector2 Size => new Vector2(Width, Height);
    
    //With pos start from topLeft
    public Rect Rect => new Rect(TopLeft, Size);
    
    #endregion


    #region Unity Method

    private void Awake()
    {
        if (isInstanceOfSingleton)
        {
            if (instance == null || instance == this)
            {
                instance = this;
                
                UpdateRealScreenSize();
                ApplyMinScreenSize();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        else
        {
            UpdateRealScreenSize();
            ApplyMinScreenSize();
        }
    }

    private void Start()
    {
        
    }
    
    private void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }

    private void Update()
    {
        if(applyMinScreenSizeInUpdate)
            ApplyMinScreenSize();
    }

    #endregion
    

    public static void UpdateRealScreenSize()
    {
        realScreenWidth = Screen.width;
        realScreenHeight = Screen.height;
    }

    public void ApplyMinScreenSize()
    {
        float ratio = realScreenWidth / realScreenHeight;
        
        //Min Width
        if (useMinWidth)
        {
            float currentWidth = Height * ratio;
            if (currentWidth < minWidth)
            {
                Width = minWidth;
            }
        }
        
        //Min Height
        if (useMinHeight)
        {
            float currentHeight = Width / ratio;
            if(currentHeight < minHeight)
            {
                Height = minHeight;
            }
        }
    }
    
    public bool IsInScreen(Vector2 position)
    {
        if (position.x < MinPosX)
            return false;
        
        if (position.x > MaxPosX)
            return false;
        
        if (position.y > MaxPosY)
            return false;
        
        if (position.y < MinPosY)
            return false;

        return true;
    }
    
    public bool IsInScreen(Vector2 position, Vector2 size)
    {
        if (position.x - size.x < MinPosX)
            return false;
        
        if (position.x + size.x > MaxPosX)
            return false;
        
        if (position.y - size.y < MinPosY)
            return false;
        
        if (position.y + size.y > MaxPosY)
            return false;

        return true;
    }
    
    public bool IsInScreen(Vector2 position, float radius)
    {
        if (position.x - radius < MinPosX)
            return false;
        
        if (position.x + radius > MaxPosX)
            return false;
        
        if (position.y - radius < MinPosY)
            return false;
        
        if (position.y + radius > MaxPosY)
            return false;

        return true;
    }
    
    public Vector2 ClampPosInScreen(Vector2 position)
    {
        position.x = Mathf.Clamp(position.x, MinPosX, MaxPosX);
        position.y = Mathf.Clamp(position.y, MinPosY, MaxPosY);

        return position;
    }
    
    public Vector2 ClampPosOutScreen(Vector2 position)
    {
        if (position.x < Center.x) //Nếu nằm về nửa bên trái
        {
            if (position.x > MinPosX)
                position.x = MinPosX;
        }
        else
        {
            if (position.x < MaxPosX)
                position.x = MaxPosX;
        }

        if (position.y < Center.y)
        {
            if (position.y > MinPosY)
                position.y = MinPosY;
        }
        else
        {
            if (position.y < MaxPosY)
                position.y = MaxPosY;
        }

        return position;
    }
    
    public Vector2 ClampPosInScreen(Vector2 position, Vector2 size)
    {
        position.x = Mathf.Clamp(position.x, MinPosX + size.x/2, MaxPosX - size.x/2);
        position.y = Mathf.Clamp(position.y, MinPosY + size.y/2, MaxPosY - size.y/2);

        return position;
    }
    
    public Vector2 ClampPosInScreen(Vector2 position, float radius)
    {
        position.x = Mathf.Clamp(position.x, MinPosX + radius, MaxPosX - radius);
        position.y = Mathf.Clamp(position.y, MinPosY + radius, MaxPosY - radius);

        return position;
    }

    public float GetMaxDistanceTo4Corner(Vector2 position)
    {
        float maxDistance = Vector2.Distance(position, TopLeft);
        
        float nextDistance = Vector2.Distance(position, TopRight);
        if (nextDistance > maxDistance)
            maxDistance = nextDistance;
        
        nextDistance = Vector2.Distance(position, BottomLeft);
        if (nextDistance > maxDistance)
            maxDistance = nextDistance;
        
        nextDistance = Vector2.Distance(position, BottomRight);
        if (nextDistance > maxDistance)
            maxDistance = nextDistance;

        return maxDistance;
    }

    public Collider2D[] OverlapInScreenAll(LayerMask layerMask)
    {
        return Physics2D.OverlapBoxAll(Center, Size, 0, layerMask);
    }
}