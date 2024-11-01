using Cysharp.Threading.Tasks;
using DG.Tweening;
using JSAM;
using Redcode.Extensions;
using TMPro;
using Toolkit.UI;
using UnityEngine;

public class CountdownUIView : UIView
{
    [SerializeField] private TextMeshProUGUI _cooldownText;
    
    public override void Show()
    {
        base.Show();
        PerformCountdown().Forget();
    }

    private async UniTaskVoid PerformCountdown()
    {
        AudioManager.PlaySound(ESound.CountSoundSO);
        await ChangeTextAnimation("3");
        AudioManager.PlaySound(ESound.CountSoundSO);
        await ChangeTextAnimation("2");
        AudioManager.PlaySound(ESound.CountSoundSO);
        await ChangeTextAnimation("1");
        AudioManager.PlaySound(ESound.StartSoundSO);
        await ChangeTextAnimation("START");
        Hide();

        async UniTask ChangeTextAnimation(string text)
        {
            _cooldownText.rectTransform.SetAnchoredPositionY(0);
            _cooldownText.rectTransform.DOAnchorPosY(100, 0.5f);
            _cooldownText.text = text;
            await UniTask.WaitForSeconds(1f);
        }
    }
}