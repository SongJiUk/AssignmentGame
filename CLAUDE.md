# CLAUDE.md — Prison Life

## 프로젝트 개요
- **장르:** 쿼터뷰 2D 캐주얼 시뮬레이션
- **Unity 버전:** 2022.3.62f2
- **해상도:** 720x1280 (Portrait)
- **플랫폼:** Android / iOS
- **언어:** C# (Unity)

---

## 코어 게임 루프
```
광물 채굴 zone → 등뒤 스택 수집
→ 기계 zone 진입 → 광물이 기계 앞으로 날아가 쌓임
→ 기계 자동 변환 (광물 → 수갑, 1개당 3초)
→ 수갑 수령 zone 진입 → 등뒤 스택에 수갑 쌓임
→ 입구 area 진입 → 수갑이 수감자 머리 위로 날아감
→ 수갑 요구량(1~5개 랜덤) 충족 → 수감자 방 배정 → 돈 획득
→ 돈으로 업그레이드 (채굴 장비 / 방 확장 / NPC 고용)
```

---

## 아키텍처 원칙

### 데이터
- 모든 게임 수치(속도, 비용, 효과값 등)는 **ScriptableObject**로 외부화
- 런타임 상태는 Manager 클래스에서 중앙 관리
- 저장은 **PlayerPrefs** (과제 범위 내 간단하게)

### 패턴
- **Object Pooling:** 수감자, 광물, 수갑 아이템, 이펙트 파티클
- **Manager 패턴:** GameManager, CurrencyManager, UpgradeManager
- **Event 기반:** UnityEvent 또는 C# Action으로 시스템 간 결합도 낮춤
- **Queue:** 수감자 대기열, 기계 변환 큐

### 성능 (모바일 기준)
- Update() 최소화 → **UniTask**로 비동기 처리
- 수감자 30명 이상 시 중앙 Manager에서 일괄 처리
- DOTween 사용 (애니메이션/트윈)

---

## 폴더 구조
```
Assets/
├── Scripts/
│   ├── Player/
│   │   ├── PlayerController.cs       # 교도관 이동
│   │   └── FollowStackSystem.cs      # 등뒤 스택 핵심 시스템
│   ├── Systems/
│   │   ├── MiningSystem.cs           # 광물 채굴
│   │   ├── MachineSystem.cs          # 광물→수갑 변환 기계
│   │   ├── PrisonerSystem.cs         # 수감자 입장/배정
│   │   └── RoomSystem.cs             # 방 수용/대기열
│   ├── NPC/
│   │   ├── WaypointPath.cs           # Waypoint 포인트 배열 관리
│   │   ├── NPCMover.cs               # Waypoint 기반 NPC 이동
│   │   └── SortingOrderUpdater.cs    # Y축 기반 Sorting Order 동적 조정
│   ├── Upgrade/
│   │   ├── UpgradeManager.cs
│   │   └── UpgradeData.cs            # ScriptableObject
│   ├── Managers/
│   │   ├── GameManager.cs
│   │   ├── CurrencyManager.cs
│   │   └── PoolManager.cs
│   ├── UI/
│   │   ├── HUD.cs
│   │   ├── PrisonerUI.cs             # 머리 위 수갑 요구량
│   │   └── UpgradeUI.cs
│   └── Data/
│       ├── MiningData.cs             # ScriptableObject
│       ├── PrisonerData.cs           # ScriptableObject
│       └── RoomData.cs               # ScriptableObject
├── Prefabs/
│   ├── Player/
│   ├── Items/                        # 광물, 수갑
│   ├── Prisoner/
│   └── UI/
├── ScriptableObjects/
│   ├── Mining/
│   ├── Upgrade/
│   └── Room/
├── Scenes/
│   └── GameScene.unity
└── Resources/
    └── Sprites/
```

---

## NPC 이동 방식

### Waypoint 방식 사용 (NavMesh 사용 금지)
- 맵 경로가 고정되어 있으므로 Waypoint가 적합
- NavMesh 대비 구현 단순, 모바일 성능 유리
- 2D 쿼터뷰에서 NavMesh 세팅 복잡도 불필요

### Waypoint 구조
```
WaypointPath (빈 GameObject)
├── Point_00 (입구)
├── Point_01 (복도)
├── Point_02 (방 앞)
└── Point_03 (방 안)
```

### NPC별 경로
- **수감자:** 입구 → 복도 → 배정된 방 (단방향)
- **보조 NPC:** 입구 ↔ 수갑 보관소 (반복 순환)
- 각 방마다 고유한 WaypointPath를 씬에 배치
- NPC는 자신에게 할당된 WaypointPath를 순서대로 이동

### 구현 규칙
- `WaypointPath.cs`: Transform[] 배열로 포인트 관리
- `NPCMover.cs`: 현재 인덱스 기준으로 MoveTowards 또는 DOTween으로 이동
- 이동 속도는 ScriptableObject로 외부화

---

## 핵심 시스템 구현 우선순위
1. **FollowStackSystem** — 등뒤 스택 (게임 feel의 핵심)
2. **ItemFlyAnimation** — 아이템 포물선 날아가기 (DOTween)
3. **PlayerController** — 교도관 이동 (쿼터뷰 조이스틱)
4. **MachineSystem** — 광물 → 수갑 변환
5. **PrisonerSystem** — 수감자 입장 + 수갑 요구량
6. **RoomSystem** — 방 배정 + 대기열
7. **UpgradeSystem** — 업그레이드 구매
8. **UI / 폴리싱**

---

## 구현 진행 현황

### 완료
- [x] **BaseController** — Init() 중복 방지 패턴
- [x] **PlayerController** — 조이스틱 이동, 애니메이션 상태 전환
- [x] **MineralZone** — 8×17 그리드 배치, 트리거 감지, 가장 가까운 Mineral 반환, 리스폰 타이머
- [x] **Mineral** — hp 관리, OnEnable() hp 리셋, Mining() 훅
- [x] **PoolManager** — Unity ObjectPool 기반 Pop/Push
- [x] **ObjectManager** — SpawnPlayer, SpawnMineral
- [x] **GameManager** — 플레이어 스폰 + 조이스틱 연결
- [x] **바닥 미네랄 리스폰** — 채굴 후 SetActive(false) → UniTask 딜레이 → SetActive(true)

### 결정 사항
- 바닥 미네랄은 풀링 없이 SetActive 토글로 처리 (위치 고정, MineralZone이 참조 보유)
- 풀링은 동적 생성 아이템에만 적용 (등뒤 미네랄 아이템, 수갑, 돈 이펙트)
- 리스폰 타이머는 MineralZone이 담당 (Mineral은 SetActive 시 꺼지므로)

### 다음 작업
- [ ] **FollowStackSystem** — 채굴 후 등뒤 스택에 미네랄 아이템 쌓기
- [ ] **ItemFlyAnimation** — 미네랄이 플레이어 등뒤로 날아가는 포물선 연출

---

## AI 활용 규칙 (Claude Code용)
- 새 시스템 작성 전 반드시 **설계 먼저 확인** 후 구현 요청
- 코드 받은 후 **반드시 로직 설명 요청** ("이 부분 왜 이렇게 했어?")
- 버그 발생 시 **에러 메시지 + 관련 코드 전체** 첨부
- 최적화 이슈 시 **Profiler 수치** 함께 제공
- 한 번에 하나의 시스템만 요청 (범위 작게)
- "코드 직접 확인해봐" 또는 이와 유사한 말을 하면 → 관련 파일을 직접 열어서 확인할 것 (코드를 붙여넣으라고 요청하지 말 것)

---

## 언어 규칙
- 모든 답변과 코드 주석은 반드시 **한국어**로 작성
- 애매하거나 추측이 필요한 경우 구현 전에 반드시 질문할 것

---

## 학습 원칙
- 코드를 바로 짜주지 말고, **접근 방법과 힌트를 먼저 제시**할 것
- 사용자가 직접 시도한 코드가 있으면 그걸 기반으로 **개선 방향만** 알려줄 것
- 왜 이렇게 구현하는지 **이유를 항상 설명**할 것
- 모르는 개념이 나오면 관련 **Unity 공식 문서 링크** 함께 제공할 것

---

## 코드 요청 시 대응 방식
- **1단계:** 문제 접근 방법 설명
- **2단계:** 사용자가 직접 구현 시도
- **3단계:** 요청 시에만 전체 코드 제공
- 단, 반복 작업 / 자동화 / 데이터 변환은 바로 코드 제공 가능

---

## 코드 리뷰 방식
- 잘된 부분 먼저 언급
- 문제점은 **왜 문제인지 이유** 설명
- 개선 방법은 **힌트로 먼저 제시**, 직접 수정은 요청 시에만

---

## 제약 사항
- 유료 에셋 사용 불가
- 무료 에셋 사용 시 출처 명시 필수 (CREDITS.txt에 기록)
- DOTween (무료 버전) 사용 가능
- Unity 기본 에셋 사용 가능
- AI 생성 리소스 사용 가능

---

## 참고 게임
- **슈퍼센트 Prison Life** (io.supercent.prison)
- 쿼터뷰 스타일, Follow Stack 메커니즘, 캐주얼 idle 루프
