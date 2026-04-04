using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class TutorialManager : MonoBehaviour
{

    [SerializeField] TutorialArrows arrow;
    [SerializeField] Transform mineralZoneTarget;
    [SerializeField] Transform machineZoneTarget;
    [SerializeField] Transform handCuffZoneTarget;
    [SerializeField] Transform prisonerZoneTarget;
    [SerializeField] Transform moneyZoneTarget;
    [SerializeField] MoneyZone moneyzone;
    Define.TutorialStep currentStep;
    PlayerController player;

    private void Start()
    {
        player = Managers.GameM.player;
        arrow.Init();
        ChangeStep(Define.TutorialStep.GoToMineralZone);
        
    }

    public void ChangeStep(Define.TutorialStep _step)
    {
        currentStep = _step;
        switch (_step)
        {
            case Define.TutorialStep.GoToMineralZone:
                arrow.SetTarget(mineralZoneTarget);
                CheckArrival(mineralZoneTarget, () => ChangeStep(Define.TutorialStep.MineMineral)).Forget();
                break;
            case Define.TutorialStep.MineMineral:
                CheckCondition(() => player.FollowStackSystem.MineralCount >= 5,
                    () => ChangeStep(Define.TutorialStep.GoToMachineZone)).Forget();
                break;
            case Define.TutorialStep.GoToMachineZone:
                arrow.SetTarget(machineZoneTarget);
                CheckArrival(machineZoneTarget, () => ChangeStep(Define.TutorialStep.GetHandCuff)).Forget();
                break;

            case Define.TutorialStep.GetHandCuff:
                arrow.SetTarget(handCuffZoneTarget);
                CheckCondition(() => player.FollowStackSystem.handCuffCount >= 1,
                    () => ChangeStep(Define.TutorialStep.GoToPrisonerZone)).Forget();
                break;

            case Define.TutorialStep.GoToPrisonerZone:
                arrow.SetTarget(prisonerZoneTarget);
                CheckArrival(prisonerZoneTarget, () => ChangeStep(Define.TutorialStep.GetMoney)).Forget();
                break;

            case Define.TutorialStep.GetMoney:
                arrow.SetTarget(moneyZoneTarget);
                CheckCondition(() => Managers.GameM.Money >= 1,
                    () => ChangeStep(Define.TutorialStep.Complete)).Forget();
                break;

            case Define.TutorialStep.Complete:
                arrow.EndTutorial();
                gameObject.SetActive(false);
                break;
        }
    }

    async UniTaskVoid CheckArrival(Transform _target, System.Action _onComplete)
    {
        await UniTask.WaitUntil(() => (player.transform.position - _target.position).sqrMagnitude < 2f);
        arrow.Hide();
        _onComplete?.Invoke();
    }

    async UniTaskVoid CheckCondition(System.Func<bool> _condition, System.Action _onComplete)
    {
        await UniTask.WaitUntil(_condition);
        _onComplete?.Invoke();
    }
}
