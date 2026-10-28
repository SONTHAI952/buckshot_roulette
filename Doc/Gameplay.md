# Gameplay Design Document — Game đấu súng may rủi theo lượt

> **Mục đích:** Tài liệu triển khai gameplay để Claude/code agent có thể xây dựng một game lấy cảm hứng từ cấu trúc và các hệ thống chơi của *Buckshot Roulette*. Tài liệu tập trung vào luật chơi, vòng lặp, trạng thái, vật phẩm, AI và tiêu chí nghiệm thu; không yêu cầu sao chép mã nguồn, lời thoại, mô hình 3D, âm thanh, nhạc, UI/art độc quyền hay nhận diện thương hiệu của trò chơi gốc.
>
> **Lưu ý về nguồn:** Chưa tìm thấy Game Design Document (GDD) đầy đủ do nhà phát triển công bố công khai. Tài liệu dưới đây là bản đặc tả triển khai được tổng hợp từ trang Steam chính thức, trang itch.io chính thức của nhà phát triển và các hướng dẫn gameplay cộng đồng. Một số chi tiết triển khai nội bộ không được công bố sẽ được ghi rõ là cấu hình đề xuất hoặc cần xác minh bằng cách chơi bản tham chiếu.

## 1. Tóm tắt sản phẩm

### 1.1. Thể loại

- Game kinh dị tâm lý, đấu trí theo lượt, quản lý xác suất và tài nguyên.
- Góc nhìn thứ nhất, camera chủ yếu cố định hướng về mặt bàn.
- Một ván chơi ngắn, tập trung vào quyết định căng thẳng hơn là thao tác nhanh.
- Gameplay lõi xoay quanh một khẩu shotgun có các viên đạn thật và đạn lép được nạp theo thứ tự ngẫu nhiên; người chơi phải suy luận thứ tự đạn, sử dụng vật phẩm và chọn mục tiêu.

### 1.2. Trải nghiệm mục tiêu

Người chơi cần liên tục tự hỏi:

- Viên đạn tiếp theo có khả năng là đạn thật hay đạn lép?
- Có nên bắn đối thủ, tự bắn để tận dụng lượt miễn phí, hay dùng vật phẩm trước?
- Có nên tiêu hao thông tin/vật phẩm ngay bây giờ hay giữ lại để tạo combo sau?
- Khi nào nên tận dụng xác suất và khi nào nên chơi an toàn?

Cảm giác chủ đạo là **ít thao tác nhưng mỗi quyết định đều có hệ quả lớn**. Ngẫu nhiên phải tạo hồi hộp, nhưng không nên khiến người chơi cảm thấy kết quả hoàn toàn vô lý: số lượng đạn từng loại phải được công khai, còn thứ tự chỉ được tiết lộ một phần bởi vật phẩm hoặc qua các viên đạn đã bắn/eject.

### 1.3. Nền tảng mục tiêu

Có thể triển khai trên PC trước. Nếu làm bản mobile, giữ nguyên hệ thống luật nhưng chuyển các thao tác chuột thành chạm, tăng kích thước vật phẩm và mục tiêu, đồng thời tránh yêu cầu kéo-thả chính xác.

### 1.4. Các chế độ cần có

1. **Story Mode:** Chơi đơn đấu với Dealer, gồm ba giai đoạn có độ khó tăng dần.
2. **Double or Nothing:** Chế độ thử thách dạng endless/run-based, có rủi ro mất toàn bộ phần thưởng đang tích lũy.
3. **Multiplayer:** Đấu nhiều người, luân phiên sử dụng shotgun, có vật phẩm riêng cho chế độ nhiều người.

Nếu cần một bản MVP để chạy gameplay trước, triển khai Story Mode trước; sau khi luật và state machine ổn định mới bổ sung Double or Nothing và Multiplayer.

---

## 2. Vòng lặp gameplay tổng quát

1. Vào phòng chơi và bắt đầu một giai đoạn.
2. Hệ thống tạo một **load** (lần nạp đạn) với số lượng đạn thật và đạn lép đã biết, nhưng thứ tự bị ẩn.
3. Game xác định người đang có lượt.
4. Người đang có lượt có thể sử dụng một hoặc nhiều vật phẩm hợp lệ.
5. Người chơi chọn mục tiêu và bóp cò.
6. Hệ thống đọc viên đạn hiện tại, áp dụng sát thương hoặc hiệu ứng đạn lép, cập nhật lượt.
7. Khi hết đạn hoặc có người hết charge theo luật của giai đoạn, game kết thúc load/giai đoạn hoặc bắt đầu lần nạp tiếp theo.
8. Phát hoạt cảnh, cập nhật trạng thái, cấp vật phẩm/charge theo giai đoạn.
9. Kết thúc giai đoạn, tính kết quả và chuyển sang giai đoạn kế tiếp hoặc kết thúc ván.

### Thuật ngữ

- **Charge:** Đơn vị máu/mạng. Đạn thật có thể làm mất charge.
- **Live shell / đạn thật:** Viên đạn gây sát thương.
- **Blank shell / đạn lép:** Viên đạn không gây sát thương.
- **Load:** Một tập các viên đạn được nạp vào shotgun trong một lần. Số lượng đạn thật và lép được công khai; thứ tự của chúng bị ẩn cho đến khi có thông tin.
- **Current shell:** Viên đạn sắp được bắn hoặc bị eject.
- **Turn:** Một lượt hành động bắn. Dùng vật phẩm không tự động kết thúc lượt, trừ khi hiệu ứng cụ thể của vật phẩm/mode nói khác.
- **Stage:** Một giai đoạn lớn trong Story Mode. Mỗi stage gồm một hay nhiều load.
- **Dealer:** Đối thủ AI trong Story Mode và Double or Nothing.

---

## 3. Luật gameplay lõi

### 3.1. Shotgun và đạn

Mỗi load gồm hai loại đạn:

- Đạn thật (live): gây **1 charge sát thương** theo luật cơ bản.
- Đạn lép (blank): gây **0 sát thương**.

Khi bắt đầu load, hệ thống phải:

1. Chọn số lượng đạn thật và đạn lép hợp lệ cho mode/stage.
2. Tạo danh sách đạn có đúng thành phần đó.
3. Xáo trộn thứ tự danh sách bằng bộ sinh số ngẫu nhiên.
4. Hiển thị số lượng đạn còn lại của từng loại nhưng không tiết lộ toàn bộ thứ tự.
5. Đặt con trỏ vào viên đầu tiên.

Sau mỗi phát bắn hoặc lần eject, lấy viên hiện tại ra khỏi danh sách và chuyển sang viên tiếp theo.

**Quy tắc bảo đảm:** Không tự ý tạo đạn mới giữa một load; chỉ nạp lại khi load cạn hoặc khi luật của mode yêu cầu. Vật phẩm eject loại bỏ viên hiện tại khỏi load, không gây sát thương.

### 3.2. Quy tắc bắn

Trong Story Mode và Double or Nothing:

| Mục tiêu | Đạn thật | Đạn lép | Quy tắc lượt |
|---|---|---|---|
| Tự bắn | Người bắn mất 1 charge (hoặc 2 nếu hiệu ứng cưa nòng đang bật) | Không mất charge | Bắn đạn thật kết thúc lượt; tự bắn trúng đạn lép cho phép người đó tiếp tục lượt |
| Bắn Dealer/đối thủ | Đối thủ mất 1 charge (hoặc 2 nếu cưa nòng đang bật) | Không mất charge | Lượt kết thúc, kể cả khi bắn trúng đạn lép |

Quy tắc quan trọng: **chỉ tự bắn trúng đạn lép mới cho lượt tiếp theo**. Bắn đạn lép vào đối thủ không giữ lượt.

Trong Multiplayer, người đang có lượt chọn bất kỳ người chơi còn sống nào làm mục tiêu, kể cả bản thân. Áp dụng quy tắc bắn tương tự: tự bắn trúng đạn lép giữ lượt; bắn người khác kết thúc lượt. Người chơi bị loại không được hành động trong phần còn lại của trận.

### 3.3. Kết thúc load và nạp lại

- Nếu vẫn còn đạn sau phát bắn/eject, tiếp tục xử lý theo quy tắc lượt.
- Khi không còn viên nào, kết thúc load.
- Đặt lại hiệu ứng tạm thời gắn với một phát bắn/load theo đúng luật vật phẩm (ví dụ hiệu ứng cưa nòng kết thúc sau phát bắn kế tiếp).
- Xóa/đặt lại trạng thái khóa lượt theo quy tắc của mode; các hiệu ứng bỏ lượt như Handcuffs phải không thể bị kéo dài vô hạn bằng cách dùng lặp.
- Nếu chưa kết thúc stage/trận, tạo load mới theo cấu hình của stage/mode và cấp vật phẩm mới nếu luật yêu cầu.

### 3.4. Hiển thị thông tin và tính công bằng

Thông tin người chơi luôn nhìn thấy:

- Số charge hiện tại của mình và đối thủ.
- Tổng số đạn thật/đạn lép còn lại trong load.
- Danh sách vật phẩm của mình; số lượng vật phẩm của đối thủ tùy cách trình bày, nhưng nếu cho phép Adrenaline cướp vật phẩm thì cần thấy loại vật phẩm hợp lệ để chọn.
- Trạng thái đặc biệt: cưa nòng, bị khóa lượt, lượt hiện tại, trạng thái bất tử/không thể hồi phục của stage cuối.

Thông tin bị ẩn:

- Thứ tự đạn chưa được tiết lộ.
- Vật phẩm/hiệu ứng bí mật nếu thiết kế có thêm; không tự thêm thông tin ẩn ngoài quy tắc.

Các thông tin mà vật phẩm tiết lộ phải gắn vào đúng viên đạn và giữ nhất quán cho đến khi viên đạn bị bắn hoặc eject. Khi viên đạn bị loại khỏi load, mọi thông tin liên quan viên đó phải được xóa.

---

## 4. Story Mode

### 4.1. Tổng quan

Story Mode là chế độ mặc định, đấu với Dealer qua ba stage. Stage 1 đóng vai trò hướng dẫn; Stage 2 mở vật phẩm cơ bản; Stage 3 là giai đoạn cuối, thất bại sẽ kết thúc run.

### 4.2. Stage 1 — Hướng dẫn

- Người chơi và Dealer bắt đầu với **2 charge** mỗi bên.
- Không có vật phẩm.
- Load đầu: **1 đạn thật + 2 đạn lép**, thứ tự ngẫu nhiên.
- Load tiếp theo được mô tả trong tài liệu gameplay cộng đồng là **3 đạn thật + 2 đạn lép**, thứ tự ngẫu nhiên.
- Mục tiêu là làm Dealer hết charge trước khi bản thân bị hạ.
- Nếu người chơi thua stage, Doctor hồi sinh người chơi và câu chuyện tiếp tục sang Stage 2; nếu thắng, chuyển thẳng sang Stage 2.

**Yêu cầu triển khai:** Stage 1 phải dạy các hành động thông qua tương tác trong game, không dùng một màn tutorial dài che mất bàn chơi. Nếu muốn giống cấu trúc bản tham chiếu, giữ đúng hai composition load nêu trên và cho phép các load sau xử lý theo quy tắc chuyển stage.

### 4.3. Stage 2 — Vật phẩm cơ bản

- Người chơi và Dealer bắt đầu stage với **4 charge** mỗi bên.
- Trước mỗi load/vòng theo luật của stage, mỗi bên nhận **2 vật phẩm ngẫu nhiên** từ pool cơ bản.
- Pool vật phẩm cơ bản: Beer, Cigarette Pack, Hand Saw, Handcuffs, Magnifying Glass.
- Load đầu của stage: **1 đạn thật + 1 đạn lép** theo thứ tự ngẫu nhiên.
- Các load tiếp theo được tạo ngẫu nhiên nhưng phải có thành phần hợp lệ và hiển thị đúng số lượng từng loại.
- Nếu người chơi thua Stage 2, Doctor hồi sinh và phạt tiền theo kết quả Stage 1: mức tham chiếu công khai là 4.000 nếu người chơi đã thắng Stage 1, hoặc 2.000 nếu đã thua Stage 1. Game vẫn tiếp tục sang Stage 3.

**Lưu ý triển khai:** Tiền phạt chỉ liên quan tới hệ thống kết quả/điểm số, không được làm thay đổi luật sát thương hoặc số đạn. Nếu dự án không cần điểm số ở bản MVP, vẫn giữ một trường `cashPenalty` để bổ sung sau.

### 4.4. Stage 3 — Sinh tử

- Là stage cuối của Story Mode.
- Người chơi và Dealer bắt đầu với **4 charge hoạt động**. Giao diện tham chiếu còn có các chỉ báo charge mờ và một hiệu ứng dây/cáp bị cắt để thể hiện trạng thái không còn hồi sinh; hãy tách trạng thái UI này khỏi số charge thực tế để tránh cộng nhầm máu.
- Trước mỗi load, mỗi bên nhận **4 vật phẩm ngẫu nhiên** từ pool vật phẩm cơ bản.
- Mỗi load có số lượng và thành phần đạn ngẫu nhiên; luôn phải có ít nhất **1 đạn thật và 1 đạn lép**.
- Khi một bên mất charge hoạt động cuối cùng, bên đó đi vào trạng thái nguy hiểm không hồi phục theo hình ảnh của stage. Với người chơi, nếu thua stage này thì run kết thúc; không được Doctor hồi sinh.
- Nếu người chơi thắng, mở phần kết quả và thưởng tiền.

**Cách mô hình hóa nên dùng:**

- `activeCharges` là lượng máu có thể chịu sát thương.
- `revivalAvailable` hoặc `fatalStage` quyết định hậu quả khi hết charge.
- `healingDisabled` bật khi hệ thống hồi sinh/defibrillator bị vô hiệu theo luật stage.
- Không coi charge mờ là máu bổ sung nếu chưa xác minh trực tiếp từ bản tham chiếu.

### 4.5. Kết quả Story Mode

- Thắng Stage 3: hiển thị kết quả run và tiền thưởng.
- Thua Stage 3: kết thúc run, hiển thị màn hình thua.
- Theo tài liệu công khai, mức tiền tối đa của Story Mode là 70.000; tiền bị trừ nếu chết, dùng thuốc lá hoặc bia. Nếu cần tái hiện chính xác scoring, hãy kiểm tra bản tham chiếu theo phiên bản hiện tại. Có thể đặt hệ thống điểm trong module riêng để cân bằng dễ dàng.

---

## 5. Double or Nothing

### 5.1. Truy cập và mục tiêu

- Trong game tham chiếu, chế độ này được mở từ khu vực Bathroom bằng cách chọn lọ thuốc trước khi rời phòng.
- Đây là chế độ dành cho người đã nắm luật cơ bản; không cần tutorial dài.
- Mục tiêu là hoàn thành các bộ ba stage liên tiếp, thu thập tiền và quyết định có tiếp tục đánh cược hay chốt kết quả.
- Không có cơ chế hồi sinh sau khi run thất bại. Thua ở bất kỳ stage nào sẽ kết thúc run và mất phần tiền đang đặt cược theo luật của mode.

### 5.2. Cấu hình một stage

Các giá trị công khai cho biết nhiều yếu tố được ngẫu nhiên hóa trong phạm vi:

- Số đạn trong một load: **2–8 viên**.
- Số vật phẩm mỗi bên: **1–5 vật phẩm**.
- Số charge khởi đầu: **2–4**; người chơi và Dealer nhận cùng số charge.
- Số lượng vật phẩm của hai bên tương đương, nhưng danh sách vật phẩm được rút riêng nên không nhất thiết giống nhau.
- Load phải có ít nhất một đạn thật và một đạn lép khi dùng quy tắc tạo load thông thường của mode.

Những giới hạn trên phải đặt trong config, không hard-code rải rác trong UI hoặc AI.

### 5.3. Pool vật phẩm

Double or Nothing dùng vật phẩm cơ bản và bổ sung:

- Adrenaline.
- Burner Phone.
- Expired Medicine.
- Inverter.

Handcuffs vẫn được dùng trong chế độ đơn; Remote/Jammer là vật phẩm nhiều người và không thuộc pool đơn mặc định.

### 5.4. Nhân đôi tiền thưởng

Sau khi người chơi hoàn tất một chuỗi ba stage:

1. Hiển thị số tiền hiện đang có thể chốt.
2. Cho hai lựa chọn: **Cash Out** hoặc **Double / Continue**.
3. Cash Out kết thúc run và lưu kết quả.
4. Continue bắt đầu chuỗi ba stage tiếp theo với mức thưởng/rủi ro được nhân đôi theo luật mode.
5. Nếu thua ở chuỗi tiếp theo, mất khoản đang đặt cược theo luật của mode; không tự động cho quay lại checkpoint.

Tách `bankedCash` (tiền đã chốt) khỏi `atRiskCash` (tiền đang đặt cược) để tránh lỗi cộng/trừ khi người chơi chọn tiếp tục.

### 5.5. Tính điểm theo thời gian

Tài liệu cộng đồng ghi nhận tiền của một set có thể giảm theo thời gian hoàn thành; chi tiết thời điểm bắt đầu/dừng đồng hồ và cách làm tròn có thể phụ thuộc phiên bản. Nếu cần tái hiện chính xác, xác minh bằng thử nghiệm game hiện tại. Với bản đầu, tách thuật toán tính thưởng thành hàm độc lập:

`calculatePayout(mode, elapsedTime, penalties, stageResults)`

Không để UI tự tính tiền bằng các phép cộng/trừ riêng lẻ.

---

## 6. Multiplayer

### 6.1. Tổng quan

- Multiplayer chính thức của bản Steam được phát hành ngày 31/10/2024.
- Thiết kế theo bàn đấu nhiều người, hỗ trợ **2–4 người chơi** theo các tài liệu hướng dẫn công khai.
- Có thể tổ chức lobby riêng/công khai và bổ sung bot nếu phạm vi dự án có AI.
- Người chơi luân phiên cầm shotgun. Khi tới lượt, người chơi có thể nhắm vào bất kỳ người chơi nào còn sống hoặc tự bắn.
- Người chơi bị loại không hồi sinh trong phần còn lại của trận.

### 6.2. Luật lượt nhiều người

- Sắp xếp người chơi sống theo vòng thứ tự lượt.
- Khi lượt bắt đầu, người chơi có thể dùng vật phẩm hợp lệ rồi chọn mục tiêu.
- Đạn thật làm mục tiêu mất charge; nếu dùng Hand Saw và đạn là đạn thật, sát thương tăng gấp đôi.
- Đạn lép bắn vào người khác kết thúc lượt.
- Đạn lép tự bắn cho người bắn thêm lượt.
- Đạn thật tự bắn gây sát thương cho bản thân và kết thúc lượt.
- Khi một người hết charge, đánh dấu `eliminated = true`; không cho họ dùng vật phẩm, trở thành mục tiêu hợp lệ hoặc nhận lượt tiếp theo.
- Nếu còn nhiều hơn một người sống, trận tiếp tục. Điều kiện chiến thắng cuối cùng nên cấu hình được; mặc định MVP có thể là người sống cuối cùng.

### 6.3. Vật phẩm riêng Multiplayer

- Handcuffs được thay bằng **Jammer** để người chơi chọn một đối thủ và bỏ lượt kế tiếp của đối thủ đó.
- **Remote** đảo chiều thứ tự lượt; chỉ có tác dụng khi số người chơi còn sống phù hợp (không có giá trị thực tế trong trận hai người).
- Có thể có Adrenaline, Burner Phone và Inverter theo pool Multiplayer.
- **Expired Medicine không nằm trong pool Multiplayer mặc định** theo tài liệu công khai.

### 6.4. Quy tắc lobby và đồng bộ

Đây là yêu cầu kỹ thuật đề xuất cho bản clone, không phải khẳng định về kiến trúc nội bộ của game tham chiếu:

- Có mã lobby, trạng thái chờ, sẵn sàng và bắt đầu trận.
- Một nguồn có thẩm quyền duy nhất quyết định thứ tự đạn, kết quả RNG, số charge và lượt hiện tại.
- Client gửi **ý định hành động** (dùng vật phẩm, chọn mục tiêu, bóp cò), không tự kết luận kết quả sát thương.
- Mọi client nhận cùng một bản cập nhật trạng thái sau mỗi hành động.
- Có timeout lượt và quy tắc xử lý người chơi rớt mạng; các quy tắc này cần được thiết kế rõ và áp dụng nhất quán.

---

## 7. Đặc tả vật phẩm

Tất cả vật phẩm tiêu hao một lần khi sử dụng thành công, trừ khi mô tả bên dưới nói khác. Thông thường, sử dụng vật phẩm không tự kết thúc lượt. Vật phẩm phải bị vô hiệu nếu không thể tạo ra hiệu ứng hợp lệ, trừ các vật phẩm có luật cho phép dùng nhưng mất tác dụng.

| Vật phẩm | Chế độ | Hiệu ứng | Quy tắc/ngoại lệ |
|---|---|---|---|
| **Magnifying Glass** | Story, Double or Nothing, Multiplayer | Tiết lộ loại viên đạn hiện tại (đạn thật/đạn lép) cho người dùng | Không tiêu thụ viên đạn. Kiến thức áp dụng đúng viên đang ở nòng; xóa khi viên đó bắn/eject. |
| **Beer** | Các mode có pool cơ bản | Eject viên đạn hiện tại khỏi shotgun mà không bắn | Cho biết viên bị eject là loại nào nếu thông tin chưa biết. Nếu eject làm hết load, lượt kết thúc theo luật tham chiếu; nếu chưa hết load, người chơi thường được tiếp tục lượt. |
| **Cigarette Pack** | Story, Double or Nothing, Multiplayer | Hồi 1 charge | Không vượt quá giới hạn charge. Không hồi máu khi hệ thống hồi phục đã bị vô hiệu ở trạng thái cuối của Story Mode. |
| **Hand Saw** | Story, Double or Nothing, Multiplayer | Đạn thật của phát bắn kế tiếp gây 2 sát thương thay vì 1 | Chỉ ảnh hưởng phát bắn kế tiếp; nếu phát đó là đạn lép, hiệu ứng kết thúc mà không gây sát thương. Sau đó shotgun trở lại trạng thái bình thường. |
| **Handcuffs** | Story, Double or Nothing | Bỏ lượt kế tiếp của đối thủ | Không cho áp dụng liên tiếp để khóa vô hạn. Trạng thái khóa phải được xóa khi đã tiêu thụ hoặc khi hết hạn theo luật load. |
| **Jammer** | Multiplayer | Chọn một đối thủ sống để họ bỏ lượt kế tiếp | Không thể chọn bản thân hoặc người đã bị loại. Có tác dụng cả trong trận chỉ còn hai người sống. |
| **Inverter** | Double or Nothing, Multiplayer | Đổi loại viên đang nằm trong nòng: live thành blank hoặc blank thành live | Không thay đổi loại các viên khác. Nếu viên đã được soi/tiết lộ, cập nhật thông tin đó ngay. |
| **Burner Phone** | Double or Nothing, Multiplayer | Tiết lộ loại và vị trí tương đối của một viên đạn trong phần load còn lại | Không tiêu thụ viên được tiết lộ. Chỉ dùng được khi số viên còn lại đủ để xác định vị trí khác viên hiện tại; nếu không đủ thì xử lý theo luật/feedback của bản tham chiếu. |
| **Adrenaline** | Double or Nothing, Multiplayer | Chọn và lấy một vật phẩm hợp lệ từ đối thủ rồi sử dụng ngay | Không thể cướp một Adrenaline khác. Sau khi kích hoạt có thời gian ngắn để chọn vật phẩm; nếu hết thời gian mà chưa chọn thì mất cơ hội/tiêu hao theo luật của mode. Phải xử lý đúng hiệu ứng của vật phẩm bị lấy. |
| **Expired Medicine** | Double or Nothing | 50% cơ hội hồi 2 charge; 50% cơ hội mất 1 charge | Kết quả ngẫu nhiên phải được quyết định một lần ở logic game và đồng bộ tới UI. Hồi máu tuân thủ giới hạn charge; nhánh mất charge vẫn có thể khiến nhân vật bị hạ. |
| **Remote** | Multiplayer (hơn 2 người) | Đảo chiều vòng lượt | Chỉ đảo hướng tuần tự; không làm thay đổi danh sách người chơi sống và không đảo lại thứ tự đạn. |

### 7.1. Luật dùng vật phẩm

- Người chơi không được dùng vật phẩm ngoài lượt của mình.
- Không thể dùng vật phẩm đã tiêu hao hoặc không có trong kho.
- Mỗi thao tác dùng vật phẩm phải được kiểm tra ở logic game trước khi trừ item.
- Đối với Adrenaline, vật phẩm bị cướp được thực thi như một lần dùng vật phẩm ngay lập tức; tránh cho hiệu ứng Adrenaline kích hoạt đệ quy.
- Các vật phẩm tác động viên đạn phải luôn tham chiếu tới `currentShellId`, không thao tác theo chỉ số mảng đã lỗi thời sau khi eject.
- Nếu hiệu ứng đang chờ (ví dụ Hand Saw) và load kết thúc mà chưa có phát bắn, quy định rõ thời điểm reset; ưu tiên mô phỏng đúng bản tham chiếu.

---

## 8. AI Dealer

### 8.1. Nguyên tắc

Dealer phải đưa ra quyết định dựa trên thông tin mà nó thực sự có, không đọc toàn bộ thứ tự đạn bị ẩn. Dealer có thể dùng xác suất và thông tin từ vật phẩm đã sử dụng, tương tự người chơi.

### 8.2. Đầu vào AI

AI nhận một bản snapshot gồm:

- Stage và mode.
- Charge hiện tại của cả hai bên.
- Số đạn thật/lép còn lại trong load.
- Thông tin về viên hiện tại nếu Dealer đã soi hoặc đã được tiết lộ.
- Các viên đã bắn/eject trong load.
- Vật phẩm của Dealer; thông tin về vật phẩm đối thủ chỉ ở mức giao diện cho phép.
- Trạng thái Handcuffs/hiệu ứng bỏ lượt, Hand Saw, healing-disabled và các effect đang tồn tại.

**Không truyền thứ tự đạn nội bộ đầy đủ vào prompt/quyết định của AI.** Nếu dùng cùng một game-state object cho UI và AI, hãy tạo một hàm `toDealerObservation()` loại bỏ dữ liệu bí mật.

### 8.3. Ưu tiên quyết định đề xuất

Đây là bộ hành vi khởi đầu; có thể điều chỉnh khi playtest:

1. Xác định liệu có thể kết thúc ván/giai đoạn bằng một phát bắn an toàn đã biết.
2. Nếu có item có thể giúp biết loại đạn hiện tại, cân nhắc Magnifying Glass hoặc Burner Phone.
3. Nếu viên hiện tại là đạn thật và mục tiêu hợp lý, cân nhắc Hand Saw trước khi bắn.
4. Nếu sức khỏe thấp và có Cigarette Pack, hồi máu khi vẫn có thể hồi.
5. Nếu có Handcuffs/Jammer hợp lệ, dùng khi việc lấy thêm một lượt có giá trị rõ ràng.
6. Nếu biết viên hiện tại là đạn lép, cân nhắc tự bắn để giữ lượt.
7. Nếu không biết viên hiện tại, so sánh tỷ lệ `liveRemaining / shellsRemaining`; xem xét máu hai bên và vật phẩm còn lại để quyết định bắn ai.
8. Dùng Beer khi eject có lợi về xác suất hoặc khi nó cung cấp thông tin chiến thuật.
9. Tránh tiêu tốn vật phẩm chỉ để tạo hiệu ứng không có lợi rõ ràng, trừ khi có nguy cơ chết ngay.

### 8.4. Không gian thông tin công bằng

Nếu Dealer dùng Magnifying Glass hoặc Burner Phone, hành vi sau đó được phép phản ánh thông tin đã biết. Nhưng AI không được gian lận bằng cách truy cập shell tiếp theo mà nó chưa tiết lộ. Trong Multiplayer, bot cũng không được đọc dữ liệu bí mật từ phía server ngoài observation được cấp.

---

## 9. UI/UX và phản hồi

### 9.1. Bố cục màn chơi

- Góc nhìn chính nhìn xuống bàn đấu; shotgun và kho đạn là trung tâm.
- Vùng của người chơi và Dealer/đối thủ phải phân biệt rõ.
- Charge hiển thị thành chỉ báo trực quan, cập nhật ngay khi bị sát thương/hồi máu.
- Số đạn thật và đạn lép còn lại luôn đọc được.
- Vật phẩm được đặt trên bàn/khu vực kho; mỗi vật phẩm có hitbox đủ lớn và phản hồi khi hover/chạm.
- Thông tin và điều khiển chỉ xuất hiện khi cần; hạn chế HUD kiểu game hành động nhiều chỉ số.

### 9.2. Trạng thái UI

Ít nhất cần có các trạng thái:

1. Menu chính.
2. Đang giới thiệu/khởi tạo stage.
3. Chờ lượt của người chơi.
4. Đang chọn hoặc xem vật phẩm.
5. Đang chọn mục tiêu.
6. Đang bắn/đang phát hoạt cảnh.
7. Đang xử lý sát thương hoặc hiệu ứng.
8. Đang nạp load mới.
9. Kết thúc stage.
10. Thắng run / thua run.
11. Màn hình nhận thưởng và lựa chọn nhân đôi.
12. Lobby Multiplayer / chờ người chơi / trận đang diễn ra / người chơi đã bị loại / kết thúc trận.

### 9.3. Hoạt cảnh và âm thanh

- Mỗi hành động có animation, sound effect và khoảng nghỉ ngắn để người chơi cảm nhận hệ quả.
- Trước khi xử lý phát bắn, khóa đầu vào để tránh gửi hai hành động cùng lúc.
- Sau animation, trạng thái game mới được xác nhận; không dùng animation làm nguồn sự thật của luật chơi.
- Dùng âm thanh khác nhau cho đạn thật, đạn lép, eject, nạp đạn, dùng item, mất charge và kết thúc stage.
- Âm thanh/nhạc, mô hình Dealer, bố cục phòng, hình ảnh, câu thoại và hiệu ứng phải là tài sản tự tạo hoặc được cấp phép; không trích xuất trực tiếp từ game tham chiếu.

### 9.4. Tối ưu thao tác

- Thao tác chính phải có phản hồi rõ và vùng bấm lớn.
- Sau khi người chơi nhấn bắn, hiển thị mục tiêu đã chọn trước hoạt cảnh nếu không phá nhịp.
- Có cách hủy chọn mục tiêu trước khi bóp cò.
- Mọi vật phẩm phải có mô tả ngắn, hiệu ứng hover/focus và dấu hiệu đã hết.
- Hỗ trợ phân biệt đạn bằng cả màu sắc và biểu tượng/văn bản, tránh phụ thuộc hoàn toàn vào màu.

---

## 10. Mô hình dữ liệu khuyến nghị

Tên trường có thể đổi theo ngôn ngữ/framework, nhưng cần có các dữ liệu tương đương.

```ts
type ShellType = "LIVE" | "BLANK";
type GameMode = "STORY" | "DOUBLE_OR_NOTHING" | "MULTIPLAYER";
type GamePhase =
  | "MENU"
  | "STAGE_INTRO"
  | "PLAYER_TURN"
  | "DEALER_TURN"
  | "ITEM_TARGET_SELECTION"
  | "SHOT_RESOLUTION"
  | "RELOAD"
  | "STAGE_RESULT"
  | "CASH_OUT"
  | "GAME_OVER"
  | "LOBBY";

interface Shell {
  id: string;
  type: ShellType;
  knownTo: string[]; // Người chơi/AI đã biết loại viên này
}

interface PlayerState {
  id: string;
  displayName: string;
  isDealer: boolean;
  charges: number;
  maxCharges: number;
  inventory: ItemId[];
  alive: boolean;
  eliminated: boolean;
  skipNextTurn: boolean;
  revivalAvailable: boolean;
  healingDisabled: boolean;
}

interface ShotgunState {
  shells: Shell[];
  currentIndex: number;
  sawedForNextShot: boolean;
  liveRemaining: number;
  blankRemaining: number;
}

interface GameState {
  mode: GameMode;
  phase: GamePhase;
  stageIndex: number;
  loadIndex: number;
  players: PlayerState[];
  turnOrder: string[];
  currentPlayerId: string;
  turnDirection: 1 | -1;
  shotgun: ShotgunState;
  bankedCash: number;
  atRiskCash: number;
  stage1Outcome?: "PLAYER_WON" | "PLAYER_LOST";
  gameOver: boolean;
  winnerId?: string;
}
```

### 10.1. Hàm logic nên tách riêng

- `createLoad(mode, stage, loadIndex, config, rng)` — tạo thành phần đạn và xáo trộn.
- `getCurrentShell(state)` — lấy viên hiện tại.
- `useItem(state, actorId, itemId, targetId?)` — xác thực và áp dụng vật phẩm.
- `fire(state, actorId, targetId)` — bóp cò và tạo một kết quả bắn duy nhất.
- `resolveShellEffect(state, shell, shooterId, targetId)` — xác định sát thương và cập nhật charge.
- `advanceTurn(state)` — chuyển lượt, xét bỏ lượt và chiều lượt.
- `ejectCurrentShell(state, actorId)` — loại bỏ viên hiện tại mà không gây sát thương.
- `startNextLoad(state)` — tạo load mới, cấp item đúng mode.
- `checkStageEnd(state)` / `checkGameEnd(state)` — xử lý điều kiện kết thúc.
- `calculatePayout(state, elapsedTime)` — tính tiền thưởng.
- `toDealerObservation(state)` — tạo thông tin hợp lệ mà AI được phép biết.

Mọi hàm trên nên là logic thuần hoặc có side-effect rõ ràng; không trộn thay đổi trạng thái với animation/UI.

---

## 11. Pseudocode cho các hành động chính

### 11.1. Bắn

```text
function fire(actor, target):
    assert game.phase is actor's turn
    assert actor is alive
    assert target is valid and alive
    assert shotgun has a current shell

    shell = consumeCurrentShell()
    damage = 1
    if shotgun.sawedForNextShot and shell.type == LIVE:
        damage = 2
    shotgun.sawedForNextShot = false

    if shell.type == LIVE:
        target.charges -= damage
        if target.charges <= 0:
            target.charges = 0
            resolveEliminationOrRevival(target)

        endCurrentTurn()
    else:
        if target.id == actor.id:
            actor keeps the turn
        else:
            endCurrentTurn()

    if load is empty:
        endLoadAndApplyReloadRules()

    checkStageAndGameEnd()
```

**Lưu ý:** `resolveEliminationOrRevival()` phải khác nhau giữa Story Stage 1/2, Story Stage 3, Double or Nothing và Multiplayer. Không dùng chung một quy tắc “hết máu là Game Over” cho mọi mode.

### 11.2. Dùng Magnifying Glass

```text
function useMagnifyingGlass(actor):
    validate actor's turn and item ownership
    shell = getCurrentShell()
    add actor.id to shell.knownTo
    consume one Magnifying Glass
    show result only to actor unless game mode explicitly reveals it publicly
```

### 11.3. Dùng Beer

```text
function useBeer(actor):
    validate actor's turn and item ownership
    removedShell = ejectCurrentShell()
    consume one Beer
    show removedShell.type according to presentation rules

    if no shells remain:
        end current load and apply the load-end turn rule
    else:
        actor continues action sequence unless the mode/reference rules say otherwise
```

### 11.4. Dùng Hand Saw

```text
function useHandSaw(actor):
    validate actor's turn and item ownership
    consume one Hand Saw
    shotgun.sawedForNextShot = true
```

### 11.5. Dùng Inverter

```text
function useInverter(actor):
    validate actor's turn and item ownership
    shell = getCurrentShell()
    shell.type = LIVE if shell.type == BLANK else BLANK
    consume one Inverter
    update liveRemaining / blankRemaining
    update all knowledge for this shell to its new type only for observers who know it
```

### 11.6. Dùng Expired Medicine

```text
function useExpiredMedicine(actor, rng):
    validate actor's turn and item ownership
    consume one Expired Medicine
    if rng.chance(0.5):
        actor.charges = min(actor.maxCharges, actor.charges + 2)
    else:
        actor.charges = actor.charges - 1
        resolveEliminationOrRevival(actor) if charges <= 0
```

---

## 12. Quy tắc ngẫu nhiên và kiểm soát lỗi

- Dùng một bộ RNG có thể seed để tái hiện bug trong quá trình debug.
- Composition đạn phải được tạo xong ở đầu load rồi mới shuffle; không chọn ngẫu nhiên live/blank lại mỗi lần bắn.
- Không cho phép load thông thường có 0 đạn thật hoặc 0 đạn lép trong những stage/mode yêu cầu cả hai loại.
- Khi dùng Inverter, cập nhật thành phần đạn còn lại ngay lập tức.
- Khi bắn/eject, cập nhật `liveRemaining`/`blankRemaining` đúng một lần; không trừ hai lần vì animation callback.
- Hành động chỉ được xử lý nếu actor đang có lượt và chưa bị loại.
- Không cho thực hiện hành động trong khi đang resolve animation, trừ khi thiết kế hỗ trợ queue có kiểm soát.
- Nếu hết load sau khi dùng Beer, không được truy cập viên tiếp theo không tồn tại.
- Nếu hai sự kiện kết thúc stage xảy ra gần nhau trong multiplayer, xử lý tuần tự trên một authoritative state để chỉ có một kết quả.
- Các nhánh RNG (đặc biệt Expired Medicine) phải được tính tại gameplay layer, không phải trong client animation.

---

## 13. Điều kiện thắng/thua

### Story Mode

- Stage 1: kết thúc khi một bên mất hết charge; nếu người chơi thua, được hồi sinh và tiếp tục Story Mode.
- Stage 2: tương tự, có tiền phạt khi người chơi thua; vẫn tiếp tục Stage 3.
- Stage 3: người chơi thắng nếu hạ Dealer trước khi bị hạ; người chơi thua sẽ kết thúc run vĩnh viễn.

### Double or Nothing

- Phải hoàn tất ba stage để có quyền chốt tiền hoặc tiếp tục.
- Thua tại bất kỳ stage nào sẽ chấm dứt run và mất tiền đang đặt cược theo quy tắc mode.
- Chốt tiền là một trạng thái kết thúc hợp lệ, không phải Game Over thất bại.

### Multiplayer

- Người hết charge bị loại, không hồi sinh trong trận.
- Điều kiện thắng nên là cấu hình của lobby (ví dụ: người sống cuối cùng hoặc tính điểm qua số stage); chốt chính xác danh sách luật dựa trên phạm vi multiplayer cần làm.
- Khi còn một người sống hoặc điều kiện thắng đã đạt, khóa hành động, công bố kết quả và trở về lobby/menu.

---

## 14. Phạm vi MVP được khuyến nghị

### MVP 1 — Gameplay đơn cốt lõi

- Một màn chơi bàn đấu.
- Story Mode đủ 3 stage.
- Dealer AI cơ bản.
- Đạn thật/đạn lép, đếm đạn, nạp lại.
- Magnifying Glass, Beer, Cigarette Pack, Hand Saw, Handcuffs.
- Chuyển lượt, charge, hồi sinh/thua/thắng.
- UI debug để nhìn shell order và state khi chạy development build; tắt ở bản phát hành.

### MVP 2 — Double or Nothing

- Pool vật phẩm mở rộng.
- Expired Medicine, Inverter, Burner Phone, Adrenaline.
- Hệ thống tiền đang đặt cược, Cash Out/Continue.
- Sinh load và charge theo cấu hình random có giới hạn.

### MVP 3 — Multiplayer

- Lobby 2–4 người.
- Đồng bộ state và turn order.
- Chọn mục tiêu bất kỳ còn sống.
- Jammer và Remote.
- Xử lý rớt mạng, timeout và thoát trận.

Không nên làm multiplayer trước khi unit test cho bắn, eject, thứ tự lượt, hiệu ứng vật phẩm và điều kiện kết thúc đã ổn định.

---

## 15. Tiêu chí nghiệm thu / test cases

### Luật bắn

- [ ] Bắn đạn thật vào Dealer trừ đúng 1 charge; lượt kết thúc.
- [ ] Bắn đạn thật vào bản thân trừ đúng 1 charge; lượt kết thúc.
- [ ] Bắn đạn lép vào Dealer không trừ charge; lượt kết thúc.
- [ ] Tự bắn đạn lép không trừ charge và giữ lượt.
- [ ] Hand Saw làm phát bắn thật tiếp theo gây đúng 2 sát thương, sau đó tự reset.
- [ ] Hand Saw không gây sát thương khi phát kế tiếp là đạn lép và không giữ hiệu ứng cho phát sau.

### Load và thông tin

- [ ] Tổng `liveRemaining + blankRemaining` luôn bằng số viên còn lại.
- [ ] Magnifying Glass tiết lộ đúng viên hiện tại mà không tiêu thụ nó.
- [ ] Beer loại bỏ đúng viên hiện tại mà không áp dụng sát thương.
- [ ] Inverter đổi đúng loại viên hiện tại và cập nhật số đếm.
- [ ] Khi viên bị bắn/eject, mọi knowledge liên quan được xóa/đánh dấu lịch sử đúng cách.
- [ ] Khi load cạn, không thể bắn hoặc eject thêm.

### Vật phẩm

- [ ] Cigarette Pack hồi đúng 1 charge, không vượt max và không hồi khi healingDisabled.
- [ ] Handcuffs/Jammer bỏ đúng lượt mục tiêu, không khóa vô hạn.
- [ ] Burner Phone hiển thị đúng loại và vị trí tương đối của viên được tiết lộ.
- [ ] Adrenaline không thể cướp Adrenaline khác và hiệu ứng của item bị cướp chỉ chạy một lần.
- [ ] Expired Medicine áp dụng đúng nhánh RNG 50/50 và xử lý được trường hợp mất charge cuối cùng.
- [ ] Remote đổi chiều lượt nhưng không làm đổi shell order.

### Mode và kết thúc

- [ ] Story Stage 1 bắt đầu bằng 2 charge mỗi bên, không có item, load đầu 1 live + 2 blank.
- [ ] Stage 2 cấp 2 item/người và bắt đầu với 4 charge mỗi bên.
- [ ] Stage 3 cấp 4 item/người, có ít nhất một live và một blank trong mỗi load.
- [ ] Story Mode phân biệt rõ hồi sinh sau Stage 1/2 với thất bại vĩnh viễn ở Stage 3.
- [ ] Double or Nothing không tự hồi sinh; thắng đủ ba stage thì hiện Cash Out/Continue.
- [ ] Multiplayer loại người chơi hết charge khỏi vòng lượt và chấm dứt trận khi đạt điều kiện thắng.

### AI và công bằng

- [ ] Dealer không biết shell order chưa được tiết lộ.
- [ ] AI không chọn mục tiêu đã bị loại.
- [ ] AI không dùng item ngoài lượt hoặc item không sở hữu.
- [ ] Cùng một seed và cùng chuỗi hành động cho ra cùng trạng thái trong chế độ kiểm thử deterministic.

---

## 16. Những điểm phải xác minh nếu cần độ chính xác tuyệt đối

Các tài liệu công khai mô tả nhiều luật cơ bản nhưng không phải là GDD chính thức đầy đủ. Trước khi chốt bản clone chi tiết, hãy tự kiểm tra trực tiếp bản Steam hiện tại để xác minh:

1. Chính xác khi nào mỗi load mới kết thúc lượt/chuyển lượt nếu Beer eject viên cuối.
2. Chi tiết cấu hình mọi load ở các stage, đặc biệt các load thứ hai trở đi.
3. Quy tắc reset Handcuffs khi reload và thứ tự ưu tiên khi một người vừa bị khóa lượt vừa tự bắn đạn lép.
4. Cách làm tròn và thời điểm chạy bộ đếm tiền trong Story/Double or Nothing.
5. Win condition, scoring và số lượng stage chính xác mà dự án muốn hỗ trợ ở Multiplayer.
6. Các animation/state chuyển tiếp phụ thuộc phiên bản.

Khi có mâu thuẫn giữa mô tả trong bài viết ở cổng game thứ ba và trang Steam/itch.io của nhà phát triển, ưu tiên nguồn chính thức và kiểm tra trực tiếp trong bản tham chiếu.

---

## 17. Nguồn tham khảo

1. **Steam — trang sản phẩm chính thức:** https://store.steampowered.com/app/2835570/Buckshot_Roulette/  
   Dùng để xác nhận nhà phát triển, chế độ chơi, mô tả chính thức và tính năng Steam/Multiplayer.
2. **Mike Klubnika — trang itch.io chính thức:** https://mikeklubnika.itch.io/buckshot-roulette  
   Mô tả thiết kế game, nhịp độ, bối cảnh và lưu ý bản itch.io không có multiplayer/leaderboard như bản Steam.
3. **Steam announcement — Multiplayer Out Now:** https://steamcommunity.com/games/2835570/announcements/detail/4477234095318892694  
   Xác nhận cập nhật Multiplayer và ngày ra mắt.
4. **Buckshot Roulette Wiki — Story Mode:** https://buckshot-roulette.fandom.com/wiki/Story_Mode  
   Tham khảo chi tiết ba stage Story Mode, charge, load và số lượng vật phẩm.
5. **Buckshot Roulette Wiki — Double or Nothing:** https://buckshot-roulette.fandom.com/wiki/Double_or_Nothing  
   Tham khảo vòng chơi endless, phạm vi ngẫu nhiên và quy tắc Cash Out/Continue.
6. **Buckshot Roulette Wiki — Multiplayer:** https://buckshot-roulette.fandom.com/wiki/Multiplayer  
   Tham khảo lượt chơi nhiều người, lobby và các vật phẩm Multiplayer.
7. **Steam Community Guide — Ultimate Guide:** https://steamcommunity.com/sharedfiles/filedetails/?id=3218902482  
   Tham khảo chiến thuật đạn/vật phẩm. Đây là hướng dẫn cộng đồng, không phải GDD chính thức.
8. **Trang bạn cung cấp:** https://buckshotroulette.com/online  
   Đây là trang bài viết của cổng game bên thứ ba; mô tả luật đặt cược trên trang không khớp đầy đủ với luật của game Steam. Không dùng bài viết đó làm nguồn chuẩn cho gameplay clone.

---

## 18. Chỉ dẫn cuối cho code agent

Hãy đọc toàn bộ file này trước khi triển khai. Ưu tiên hoàn thiện **gameplay logic thuần, state machine và test tự động** trước art/animation. Tất cả xác suất, số charge, composition load, item pool, giá trị tiền và rule theo mode phải đặt trong cấu hình có thể điều chỉnh. Dùng tài sản tạm do dự án tự tạo; không chép code, mô hình, texture, âm thanh, nhạc, lời thoại hoặc màn hình nguyên bản của *Buckshot Roulette*. Khi luật chưa được nguồn công khai xác định rõ, ghi chú giả định trong code và giữ nó dễ sửa thay vì tự gắn cứng vào nhiều nơi.
