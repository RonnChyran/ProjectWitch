using UnityEngine;
using UnityEngine.UI;

using System.Collections;

public class SoldierEditWindow : MonoBehaviour {

    [Header("スライダー")]
    [SerializeField]
    private Slider mSliderHP = null;
    [SerializeField]
    private Slider mSliderSoldier = null;
    [SerializeField]
    private Slider mSliderMaxSoldier = null;

    [Header("ステータス表示テキスト")]
    [SerializeField]
    private Text mHPText = null;
    [SerializeField]
    private Text mSoldierText = null;
    [SerializeField]
    private Text mMaxSoldierText = null;

    [Header("コスト表示テキスト")]
    [SerializeField]
    private Text mHPCostText = null;
    [SerializeField]
    private Text mSoldierCostText = null;
    [SerializeField]
    private Text mSoldierMaxText = null;

    [Header("マナ表示テキスト")]
    [SerializeField]
    private Text mRequiredManaText = null;
    [SerializeField]
    private Text mCurrentManaText = null;

    //ID
    public int UnitID { get; set; }

	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void SetText()
    {
        var game = Game.GetInstance();
        var unit = game.UnitData[UnitID];

        //スライダーの範囲を設定
        var hpmax = unit.MaxHP - unit.HP;
        if (hpmax == 0) mSliderHP.interactable = false;
        else mSliderHP.interactable = true;

        var solMax = unit.MaxSoldierNum - unit.SoldierNum;
        if (solMax == 0) mSliderSoldier.interactable = false;
        else mSliderSoldier.interactable = true;

    }
}
