1, Thông tin project
Làm bằng Unity.
Màn hình dọc hoặc ngang tuỳ build platform window hay android hoặc iOS.
Sử dụng Both Input System
Góc nhìn: chiều x là chiều ngang, chiều z là chiều dọc, chiều y là chiều sâu
Sử dụng giao thức MCP with Unity, nếu chưa kết nối thì tạm dừng việc xử lý lại và hỏi ý kiến tôi để có thể tiếp tục thực thi.


2, Cấu trúc project
Mọi thứ liên quan tới gameplay này đặt trong Assets/Game/Primary/.

Trong thư mục này đã chia sẵn các folder phù hợp từng chức năng.

Mỗi thành phần có thể chia ra thành từng System, ví dụ BlockSystem, BlockBoardSystem, BoardSystem,...
Mỗi System như vậy được đặt trong thư mục Assets/Game/Primary/System/.
Trong mỗi thư mục System này có thể có đầy đủ các thư mục Code, Prefab, Media liên quan tới System đó.


Với GamePlaySystem, đây là nơi đặt GamePlayCore, các Service liên quan tới logic gameplay, GlobalEvent liên quan tới gamePlay...


Làm vào scene CampaignMode.


3, Cấu trúc scene
Cấu trúc object cơ bản đã đặt trong scene CampaignMode

Mọi thứ thuộc về GamePlay đều đặt trong object GamePlayCore.
GamePlayCore nhận LevelData, init và chơi hoàn chỉnh.


4, GamePlayCore
GamePlayCore ref tới các Module, Service, đóng vai trò như điểm truy cập trung tâm.

GamePlayCore có event start/win/lose để CampaignMode nhận event đó và show popup win, lose.

Làm như vậy thì có thể tạo chế độ TryPlayMode dùng cho xếp level.


5, Module
Các object của gameplay nên có prefab để spawn ra, ví dụ block, boardCell...
Nếu không tạo được prefab trong project thì tạo object trong scene và deactive đi để dùng như một prefab.



6, Service
Mỗi module chỉ nên tự xử lý logic của bản thân và các logic liên quan tới child của nó.
Ví dụ: BlockBoard sẽ chỉ xử lý chính nó và các Block là child của nó.

Còn việc tương tác giữa các module, Input, logic gameplay, win, lose... hãy tạo các Service để xử lý


7, Flow Init
Khi level được load, CampaignMode lấy số level hiện tại -> tìm ra file json -> parse ra LevelData, gọi GamePlayCore.Init(levelData).
Sau khi Init thành công, CampaignMode gọi gamePlayCore.StartPlay() để bắt đầu play.


8, Camera
Dùng 2 camera, áp dụng cho MỌI scene:

- MainCamera: render các gameObject của thế giới. Culling mask bỏ layer UI. Depth = 0.
- GUICamera: chỉ render gameObject có layer UI. Culling mask chỉ có layer UI.
  Clear flags = Depth only (để giữ nền của MainCamera). Depth = 10 nên vẽ đè lên MainCamera.
  Không gắn AudioListener (mỗi scene chỉ được một cái, đã nằm ở MainCamera).
  Tôi đã có sẵn prefab camera này ở "Assets/ZeroX/Core/UI/GUICamera/Implement/Resources/ZeroX/GUICamera/GUI Camera.prefab", bạn hãy kéo lên scene và dùng thôi


9. Các Popup
- Các popup ( ví dụ PopupWin, PopupLose, Popup Setting,.. và toàn bộ các popup khác) được đặt ở bên trong "Assets/Game/Primary/UI"
- Cấu trúc folder của mỗi popup bên trong thư mục UI: folder tên popup đó => Code, Media, Resources
- Các script đặt trong thư mục Code
- Đặt tên prefab của popup đó giống hệt tên script, và đặt ở trong folder "Resources/Window/" bên trong folder của popup đó.
- Các asset thì đặt trong thư mục Media
- popup luôn kế thừa WindowController, và dùng các hàm WhenAwake (thay thế Awake), WhenStart(thay thế Start), ovveride Window_OnOpen() các hành động xảy ra khi mở popup này.
- popup được gọi qua lệnh  (ví dụ với PopupWin): PopupWin.Instance.Open();
- prefab của popup luôn chứa: canvas với các cài đặt giống UICanvas phía trên, canvasgroup, graphicRaycaster, canvasScaler để thông số giống UICanvas, component SetGUICameraForCanvas ở "Assets/ZeroX/Core/UI/GUICamera/Core/SetGUICameraForCanvas.cs", component Window ở "Assets/ZeroX/Core/UI/WindowSystem/Core/Code/Window.cs"
- cấu trúc prefab của popup: tên popup => Background + Content (kéo content vào reference content của class Window)

- Tham khảo PopupIntroduceFeature tôi tạo sẵn ở "Assets/Game/Primary/UI/Popup Introduce"

10. Camera fit
- Camera phải xử lý fit để nhìn được toàn bộ board

11. Font
- Dùng font cho tất cả: Assets/Game/Primary/UI/font/HVD/HVD Fonts  MikadoBlack SDF.asset

12. Button

UICanvas:
- Render mode: Screen Space - Camera, Render Camera = UICamera, Plane Distance = 10.
- Canvas Scaler:
    Scale mode: Scale With Screen Size
    Reference resolution: 1080 x 2400
    Screen match mode: Expand
	
- Tất cả button trong game do bạn tạo ra thì dùng ZButton ở "Assets/ZeroX/Core/UI/ButtonSystem/Code/ZButton.cs"
- Các Button xem quảng cáo thì sẽ dùng AdsButton của tôi.

13. CampaignMode
Các thành phần gameObject cở bản trong scene Campaign Mode là:
- "GUI Camera" ở "Assets/ZeroX/Core/UI/GUICamera/Implement/Resources/ZeroX/GUICamera/GUI Camera.prefab"
- "Audio Manager" ở "Assets/ZeroX/Core/AudioSystem/Implement/Audio Manager.prefab"
- "Directional Light" tạo sẵn trên scene
- "EventSystem" tạo sẵn trên scene
- "Feature Manager" ở "Assets/Game/Primary/UI/Popup Introduce/Prefab/FeatureManager.prefab" (Nếu đã import Popup Introduce này rồi, còn nếu chưa import thì bỏ qua)
- gameObject "CampaignMode", chứa bên trong: 
  +> GamePlayCore: Cơ bản sẽ bao gồm các thành phần sau, tùy thuộc vào từng game
     + Enviroment:
	 + Modules: Chia các thành phần trong màn Gameplay thành các Modules, ví dụ BoardView, DockView,...
	 + Camera Controller: chứa MainCamera bên trong để dù đưa GamePlayCore sang scene khác thì vẫn hoạt động đúng
	 + Services: Chứa các service trong gameplay như GameplayInput, AudioService, HapticService, LoseChecker, WinChecker,VfxService, SfxService...
	 + UI: chứa các thành phần UI có trên màn Gameplay mà không bao gồm các Popup, ví dụ như nút setting, hiển thị Level, thanh Coin Bar, 		Booster Frame,... nói chung là các ui thuộc về màn Gameplay chính và không bao gồm các Popup, vì các Popup được sinh ra ở từ 		Folder Resources.
	 + ...
  +> Tutorials: chứa các thông tin về Tutorial System ở đây
  
 
 14. SaveGame
 - Tôi đã thêm hệ thống SaveGame System ở "Assets/ZeroX/Core/SaveGameSystem"
 - Hãy dùng hệ thống này để lưu dữ liệu cho game
 - Cách dùng giống CampaignModeDI ở "Assets/Game/Primary/System/CampaignModeSystem/Code/CampaignModeDI.cs" 
 Cụ thể là tạo 1 class có đuôi DI cho các tính năng cần lưu dữ liệu, trong class đó sẽ có 1 root_key để định danh node dữ liệu của tính năng đó, tạo RootNode và thao tác dữ liệu qua RootNode cho tính này. Đọc CampaignModeDI để hiểu.