using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UIPlayerLable : MonoBehaviour
{
    [SerializeField] private Text fragText;
    [SerializeField] private Text nicknameText;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Color selfColor;

    private int netId;
    public int NetId => netId;

    public void Init(int netId, string nickname)
    {
        this.netId = netId;
        nicknameText.text = nickname;

        if(netId == Player.Local.netId)
        {
            backgroundImage.color = selfColor;
        }
    }

    public void UpdateFrag(int frag)
    {
        fragText.text = frag.ToString();
    }
}
