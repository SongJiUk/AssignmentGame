# 🏢 Prison Life

> 광석 채굴부터 감옥 업그레이드까지 자동화되는 아이들 경영 시뮬레이션

**개발 기간:** 2026.03 ~ 04 (약 1주일)  
**개발 인원:** 1인  
**플랫폼:** PC (Unity 2022.3 LTS)

<br>

## 🎮 게임 플로우

```
광부 → 광석 채굴
       ↓
   수갑 제작기 → 수갑 생성
                  ↓
              간부 → 수감자 구역으로 운반
                        ↓
                    수감자 수용 → 수입 발생
                                    ↓
                              무기 업그레이드 / 광부·간부 고용 (자동화 확장)
                                    ↓
                              감옥 정원이 가득 차면 → 감옥 업그레이드 (수용 인원 증가)
```

<br>

## ⚙️ 핵심 시스템

### UniTask 기반 NPC 자동화 루프
코루틴 없이 `async/await` 패턴으로 NPC의 이동·대기·전달을 처리합니다.

```csharp
async UniTaskVoid AsyncWalkJailer()
{
    while (true)
    {
        if (handCuffStack.Count == 0)
        {
            await AsyncMoveToPosition(handCuffZone.transform.position);
            await UniTask.WaitUntil(() => handCuffStack.Count >= HandCuffLine);
        }

        await AsyncMoveToPosition(prisonerZone.transform.position);
        await UniTask.WaitUntil(() => handCuffStack.Count == 0);
        await UniTask.WaitUntil(() => prisonerZone.HandCuffCount == 0);
    }
}
```

### 감옥 업그레이드 시스템
감옥 정원이 가득 차면 비용을 지불해 수용 인원을 늘릴 수 있습니다.
업그레이드할수록 수감자 수용량과 수입이 함께 증가합니다.

### IHandCuff 인터페이스 설계
`IHandCuffGiver` / `IHandCuffReceiver`를 분리해 NPC 간 직접 참조 없이 수갑 전달 시스템을 구현합니다.

```csharp
public interface IHandCuffGiver
{
    void GiveHandCuff(IHandCuffReceiver receiver);
}

public interface IHandCuffReceiver
{
    void ReceiveHandCuff(int amount);
}
```

### DOTween 스폰 연출
광석·수갑·돈이 구역으로 날아가는 연출. ObjectPool + `OnComplete` 콜백으로 애니메이션 완료 후 Release 처리합니다.

### ScriptableObject 데이터 관리
방(RoomData)·구매구역(PurchaseZoneData)·무기(WeaponData)를 ScriptableObject로 분리해 MonoBehaviour 하드코딩을 제거했습니다.

### ObjectPool (Unity 2021+)
반복 생성 오브젝트를 `ObjectPool<T>`로 재사용해 GC 부하를 최소화합니다.

<br>

## 🐛 트러블슈팅

### Stack 시각 버그 — 빈 슬롯이 잠깐 보이는 문제

**현상:** DOTween 애니메이션이 끝나기 전에 `Pop()`이 호출되어 맨 아래 슬롯이 순간적으로 비어 보임

**임시 해결:** 게임 시작 시 `Delay(500ms)` 대기 추가

**근본 해결책:**
```csharp
// 애니메이션 완료 후 Pop 호출
await obj.transform.DOMove(target.position, 0.4f).AsyncWaitForCompletion();
_stack.Pop();
```

<br>

## 📁 폴더 구조

```
Assets/Scripts/
├── Controller/     # JailerController, MiningWorkerController, NPCController, ...
├── Manager/        # Managers, GameManager, PoolManager, UIManager, ...
├── Data/           # RoomData, PurchaseZoneData, WeaponData (ScriptableObject)
└── Interface/      # IHandCuffReceiver, IHandCuffGiver
```

<br>

## 🛠 기술 스택

![Unity](https://img.shields.io/badge/Unity-000000?style=flat-square&logo=unity&logoColor=white)
![C#](https://img.shields.io/badge/C%23-239120?style=flat-square&logo=csharp&logoColor=white)
![UniTask](https://img.shields.io/badge/UniTask-a78bfa?style=flat-square)
![DOTween](https://img.shields.io/badge/DOTween-ff6b6b?style=flat-square)
