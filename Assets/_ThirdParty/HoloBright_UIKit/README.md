# Holo-Bright UI Kit (Unity UI Toolkit)

밝은 홀로라이브 톤(스카이블루 + 화이트 + 핑크/민트/옐로 포인트)의 **Flat 2.0 + 소프트 글로스** 스타일 UI 키트입니다.
모든 스프라이트는 **흰색 + 알파**로만 되어 있어 색은 전부 엔진에서 tint로 지정합니다.

## 폴더
- `Sprites/` — 55개 PNG (Sprite, 9-slice 보더 값은 `sprite_meta.json` 참고)
- `UI_Toolkit/HoloTheme.uss` — 팔레트 변수 + 모든 컴포넌트 스타일 + 테마 스왑 클래스
- `UI_Toolkit/HoloSample.uxml` — 조립 예시
- `Shader/HoloUIMask.shader` — 채널 패킹 마스크(`Panel_Mask_RGBA.png`)로 색·질감을 마테리얼에서 바꾸는 URP UI 셰이더
- `Preview.png` — 동일 스프라이트를 tint만 바꿔 합성한 미리보기

## 임포트 설정 (PNG 공통)
Texture Type: Sprite (2D and UI) / Sprite Mode: Single / Mesh Type: Full Rect /
Filter: Bilinear / Compression: None 또는 High Quality / Generate Mip Maps: Off
Sprite Editor에서 Border(L,B,R,T)를 `sprite_meta.json`의 `slice` 값 [L,R,T,B] 로 입력.
UI Toolkit은 USS의 `-unity-slice-*`가 우선이라 Sprite Editor 보더는 생략해도 됩니다 (UGUI 겸용이면 입력).

## 레이어 조립 규칙
- **Base**: 면. tint = 역할 컬러.
- **Outline**: 테두리. Base 위에 `position:absolute` 로 겹침. tint = 메인 컬러 (focus 시 변경).
- **Gloss**: 상단 광택. tint = 흰색 alpha 0.3~0.6. pressed 상태에서 alpha 0.
- **Shadow**: Base보다 8px 큰 형제 요소를 뒤에. tint = 메인 컬러 어두운 톤 alpha 0.25~0.35.
- **Pressed_Inner**: 눌림 상태 전용 오버레이. tint = 어두운 톤.
- **Pill / Circle / Tab / Tooltip**: 중앙 스트레치 영역이 있으므로 가로만 늘리기 권장 (세로는 보더 합 이상).

## 색 / 질감 변경 방법 (3단계)
1. **USS 변수** — `:root`의 `--ui-primary` 등만 바꾸면 전체 변경. `.theme-pink/.theme-mint/.theme-violet` 클래스를 루트에 붙이면 즉시 스왑.
2. **요소별 tint** — `-unity-background-image-tint-color`.
3. **마테리얼(질감)** — `Panel_Mask_RGBA.png` + `HoloUIMask.shader`: 면색/테두리색/광택색/노이즈 강도/채도를 마테리얼 파라미터로. (UI Toolkit은 런타임 커스텀 셰이더 직접 지원이 제한적이라 UGUI Image, 또는 RenderTexture 경유로 사용)

## 미지원 / 주의
- 노이즈 텍스처는 `_NoiseTiling`으로 반복되므로 9-slice 해도 늘어나지 않음 (질감 유지).
- 블러(글래스) 효과는 포함하지 않음 — 필요 시 별도 런타임 블러 패스.
- 폰트는 포함하지 않음. 둥근 고딕(예: Pretendard Rounded, M PLUS Rounded 1c)이 톤에 맞습니다.
