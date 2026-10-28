# Theme 18+ — Bảng chuyển đổi từ Buckshot Roulette

Luật chơi giữ nguyên theo `Gameplay.md`. Chỉ đổi phần trình bày (tên, model, câu chữ). Trong code, logic dùng tên trung lập
(`Shell`, `Live/Blank`, `Charges`, `ItemId` theo chức năng) để dễ đối chiếu với `Gameplay.md`; phần theme nằm trong
`GameplayConfig` (tên hiển thị, mô tả, màu, model) nên có thể đổi mà không động vào luật.

Tên game tạm: **Tease Roulette**.

## Thành phần chính

| Buckshot Roulette | Bản 18+ | Ghi chú |
|---|---|---|
| Shotgun | **Love Wand** (máy massage dạng wand) | Chĩa vào đối thủ hoặc bản thân rồi "bóp cò" |
| Live shell | **HOT charge** (viên nạp màu hồng, icon tim lửa) | Trúng thì mất 1 Stamina |
| Blank shell | **COLD charge** (viên nạp màu xanh nhạt, icon bông tuyết) | Không mất gì |
| Charge (máu) | **Stamina** (hiển thị trái tim trên Stamina Monitor) | Về 0 = gục ngã/thua stage |
| Defibrillator box | **Stamina Monitor** (hộp LCD hồng) | |
| Dealer | **Mistress** (nhân vật nữ, model add sau) | Có sẵn điểm gắn model + hook animation |
| Doctor hồi sinh | **Safe Word** | Stage 1–2: thua thì dùng safe word, chơi tiếp |
| Stage 3 cắt dây defib | **No Safe Words** | Stamina ≤ 2 ở stage cuối: không hồi máu được |
| Cash | **Tips ($)** | |

## Vật phẩm

| ItemId (code) | Gốc | Bản 18+ | Hiệu ứng |
|---|---|---|---|
| `Peek` | Magnifying Glass | **Lace Mask** | Xem viên hiện tại là HOT hay COLD |
| `Eject` | Beer | **Champagne** | Bật nút chai, đẩy viên hiện tại ra không bắn |
| `Heal` | Cigarette Pack | **Ice Cube** | Hồi 1 Stamina (không hồi khi bị khoá hồi máu) |
| `DoubleDamage` | Hand Saw | **Leather Whip** | Phát HOT kế tiếp gây 2 sát thương |
| `SkipTurn` | Handcuffs | **Fluffy Handcuffs** | Đối thủ mất lượt kế tiếp |
| `Steal` | Adrenaline | **Collar & Leash** | Cướp 1 item của đối thủ và dùng ngay |
| `RevealRandom` | Burner Phone | **Love Dice** | Tiết lộ loại + vị trí 1 viên ngẫu nhiên phía sau |
| `GambleHeal` | Expired Medicine | **Mystery Pill** | 50% hồi 2, 50% mất 1 Stamina |
| `Invert` | Inverter | **Hot Wax Candle** | Đảo viên hiện tại HOT ↔ COLD |

Pool cơ bản (Story Mode): Lace Mask, Champagne, Ice Cube, Leather Whip, Fluffy Handcuffs.
Pool Double or Nothing: pool cơ bản + Collar & Leash, Love Dice, Mystery Pill, Hot Wax Candle.

## Phạm vi đã làm

- Story Mode (3 stage) và Double or Nothing (Cash Out / Continue).
- Multiplayer chưa làm (cần networking, theo `Gameplay.md` mục 14 thì làm sau cùng).

## Cấu trúc (Assets/Game/Primary)

| Thư mục | Nội dung |
|---|---|
| `System/GamePlaySystem/Code/Rules` | Luật chơi thuần C# (asmdef riêng `Game.Primary.Rules`): `GameRules`, `GameState`, `DealerBrain` (AI), `PayoutCalculator`, `LevelData` |
| `System/GamePlaySystem/Code/Core` | `GamePlayCore` (CS `SingletonBind`) – Init(levelData) / StartPlay() / OnWin / OnLose |
| `System/GamePlaySystem/Code/Services` | `TurnFlowService` (state machine stage → load → lượt), `PlayerInputService`, `SfxService`, `MusicService`, `HapticService`, `CameraController` |
| `System/GamePlaySystem/Code/UI` | `GameHud` (UI màn chơi), `GameToast` (bọc ToastSingle) |
| `System/{Table,Wand,Stamina,Mistress}System` | Module hiển thị: bàn + item, Love Wand + viên nạp, Stamina Monitor, Mistress |
| `System/CampaignModeSystem` | `CampaignMode` (CS `SingletonBind`), `LevelLoader` |
| `System/RunSystem` | `RunDI` – lưu bằng SaveGame (`root_key = teaseRoulette`) |
| `System/PlatformLayoutSystem` | Dọc/ngang: `CanvasScalerOrientation`, `RectOrientationLayout`, editor `PlatformOrientationSwitcher` |
| `UI/Popup*` | PopupAgeGate, PopupHome, PopupWin, PopupLose, PopupDoubleOrNothing, PopupPause (WindowController) |
| `Resources/Levels` | `level_1.json` = Story, `level_2.json` = Double or Nothing |
| `Tests/EditMode` | 31 unit test cho luật chơi + AI |
| `Scene/CampaignMode.unity` | Scene chính (đã thêm vào Build Settings index 0) |

Menu `Tools/Tease Roulette/...` dựng lại toàn bộ texture, material, prefab item, popup và scene từ code
(`System/GamePlaySystem/Editor/TeaseRouletteBuilder*.cs`). Chạy lại "Build Scene" sẽ ghi đè scene CampaignMode.

## Thay model nhân vật nữ (Mistress)

1. Trong scene: `CampaignMode/GamePlayCore/Modules/Mistress/Model Root`.
2. Kéo model vào `Model Root` (nhân vật nhìn về +Z local của Mistress, ngồi sau bàn, tay đặt lên bàn ở khoảng z ≈ 0.44).
3. Tắt hoặc xoá `Placeholder (replace me)`.
4. Trên component `MistressView`: gán `animator`, gán lại `flashRenderers` (các renderer sẽ loé hồng khi bị trúng).
5. Animator Controller dùng các Trigger: `Idle`, `Hit`, `Down`, `Revive`, `UseItem`, `Shoot`, `Cuffed` (thiếu trigger nào thì bỏ qua),
   hoặc nối thêm qua các UnityEvent `onHit`, `onDown`, `onRevive`, `onUseItem`, `onShoot`, `onCuffed`.
6. Nếu làm lại scene bằng menu Build Scene thì phải gắn lại model (builder chỉ tạo placeholder).

## Ghi chú

- Input: project đặt `Active Input Handling = Both` theo Dev.md (ZeroX `ZInput` dùng `UnityEngine.Input`). Cần restart Unity để có hiệu lực.
- URP: GUI Camera của ZeroX được chuyển thành Overlay và add vào stack của Main Camera lúc chạy (`CameraController`).
- Prefab Toast mặc định của ZeroX thiếu ref `content/canvas/canvasGroup` trong `Window` → đã gán lại (menu `Fix ZeroX Toast Prefabs`).
- Font trong Dev.md (`HVD Fonts MikadoBlack SDF`) không có trong project → đang dùng LiberationSans SDF mặc định của TMP.
- SFX/nhạc hiện là âm thanh tổng hợp tạm (`Media/Audio`), thay file wav là xong (AudioAsset giữ nguyên).
- Ảnh chụp gameplay khi test: `Doc/Screenshots`.

## Giả định (chưa xác minh được với bản gốc)

Các giá trị dưới đây nằm trong config, sửa được không cần đụng code:

- Người chơi luôn đi trước ở đầu mỗi load (`playerStartsEachLoad`).
- Còng tay và hiệu ứng Whip bị reset khi hết load.
- Người vừa bị còng xong không thể bị còng lại cho tới khi họ được đi một lượt (chống khoá vô hạn).
- Tiền phạt Story: dùng safe word ở stage 2 trừ 4.000 (nếu đã thắng stage 1) hoặc 2.000; mỗi Ice Cube trừ 220; mỗi Champagne trừ 495.
- Stage cuối khoá hồi máu khi Stamina ≤ 2.
