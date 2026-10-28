1, Tổng quan
System này là công cụ để giúp xếp level một cách trực quan.
System này chỉ làm khi tôi yêu cầu làm scene Design Level, không tự ý làm cùng khi làm Gameplay

Không dùng giao diện editor của Unity, làm trực tiếp trên scene như gamePlay, tôi sẽ Play lên để xếp level. Như vậy sẽ trực quan và tiện lợi hơn.
Các object có thể đặt trong world, không cần phải là canvas.

Đặt System này trong thư mục Assets/Game/Primary/System/LevelDesignSystem.

Làm trong scene Assets/Game/Primary/System/LevelDesignSystem/Scene/Level Design.

Hãy tạo các module riêng với đuôi LD để sử dụng cho level design để tránh ảnh hưởng tới GamePlay chính.

Ví dụ gameplay chính ta có:
BlockBoard.cs
BlockBoard prefab

Block.cs
Block prefab

thì tương ứng trong Level Design Tool ta có:
BlockBoardLD.cs
BlockBoardLD prefab

BlockLD.cs
BlockLD prefab


2, Camera
Khi nhấn giữ Ctrl sẽ là chế độ chỉnh camera. Trong khi đang giữ ctrl:
Bấm chuột trái và drag sẽ drag camera.
Scroll chuột sẽ là thu phóng camera.

Trong khi đang trong chế độ điều chỉnh camera thì sẽ không thao tác với bất cứ object nào khác.


3, UI Open, Save level
Trên top, có một TMP_InputField để nhập số level.
Có button Open để load level đã nhập, bên cạnh là button Save để save lại level đã xếp.
Hai bên trái phải của ô nhập level là nút prev và next level, có tác dụng Open prev hoặc Open next level.


4, UI
Các UI khác tùy vào game mà thiết kế
