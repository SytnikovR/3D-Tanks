using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHitResultPanel : MonoBehaviour
{
    [SerializeField] private Transform spawnPanel;
    [SerializeField] private UIHitResultPopup hitResultPopup;

    private void Start()
    {
        NetworkSessionManager.Match.MatchStart += OnMatchStart;
    }

    private void OnDestroy()
    {
        NetworkSessionManager.Match.MatchStart -= OnMatchStart;
        Player.Local.ProjectileHit -= OnProjectileHit;

    }

    private void OnMatchStart()
    {
        Player.Local.ProjectileHit += OnProjectileHit;
    }

    private void OnProjectileHit(ProjectileHitResult hitResult)
    {
        if (hitResult.Type == ProjectileHitType.Environment || hitResult.Type == ProjectileHitType.ModulePenetration ||
            hitResult.Type == ProjectileHitType.ModuleNoPenetration) return;

        UIHitResultPopup hitPopup = Instantiate(hitResultPopup);
        hitPopup.transform.SetParent(spawnPanel);
        hitPopup.transform.localScale = Vector3.one;
        hitPopup.transform.position = Camera.main.WorldToScreenPoint(hitResult.Point);

        if (hitResult.Type == ProjectileHitType.Penetration)
            hitPopup.SetTypeResult("Пробитие!");

        if (hitResult.Type == ProjectileHitType.Ricochet)
            hitPopup.SetTypeResult("Рикошет!");

        if (hitResult.Type == ProjectileHitType.Nopenetration)
            hitPopup.SetTypeResult("Броня не пробита!");

        hitPopup.SetDamageResult(hitResult.Damage);
    }

}
    
