# LevelData

Level lưu dạng **json trong Resources**: `Assets/Game/Primary/Resources/Levels/level_{n}.json`.
`LevelLoader.Load(n)` đọc bằng `Resources.Load<TextAsset>` rồi `JsonUtility.FromJson<LevelData>`, sau đó
`GamePlayCore.Init(levelData)` → `StartPlay()`.

