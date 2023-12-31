using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Inven_Slot : UI_Base
{
    enum GameObjects
    {
        EquipCheck
    }

    enum Images
    {
        Highlight_Front,
        Highlight_Back,
        ItemIcon,
    }

    [HideInInspector] public Item_Equip item_Equip = null;

    [HideInInspector] public int slotNumber;

    [HideInInspector] public UI_Popup_Inven uI_Popup_Inven;
    void OnEnable()
    {
        Initialize();
    }


    public override bool Initialize()
    {
        if (!base.Initialize()) return false;

        BindObject(typeof(GameObjects));
        BindImage(typeof(Images));

        GetImage((int)Images.ItemIcon).gameObject.BindEvent(null, OnButtonEnter, Define.UIEvent.PointerEnter);
        GetImage((int)Images.ItemIcon).gameObject.BindEvent(null, OnButtonExit, Define.UIEvent.PointerExit);
        GetImage((int)Images.ItemIcon).gameObject.BindEvent(OnButtonClick);

        GetImage((int)Images.Highlight_Back).gameObject.SetActive(false);
        GetImage((int)Images.Highlight_Front).gameObject.SetActive(false);

        Refresh();

        return true;
    }

  public void Refresh()
    {

        GetImage((int)Images.ItemIcon).sprite = Managers.Data.items_Equip[slotNumber].IconImage;
        GetImage((int)Images.ItemIcon).color = (Managers.Data.userData.invenGetArray[slotNumber]) ? Color.white : Color.gray;
        GetObject((int)GameObjects.EquipCheck).SetActive(Managers.Data.userData.isWearArray[slotNumber]);

    }

    void OnButtonClick()
    {
        if (!Managers.Data.userData.invenGetArray[slotNumber]) return;

        if (Managers.Data.userData.isWearArray[slotNumber])
            UnEquip();
        else
            Equip();
    }


    public void Equip()
    {
        Managers.Data.userData.isWearArray[slotNumber] = true;
    
        Managers.Data.SaveUserDataToJson();
        Refresh();
        Managers.Game.OnEquipChanged?.Invoke();
        Managers.Game.HpBarOnly?.Invoke();

        Debug.Log($"{Managers.Data.items_Equip[slotNumber].name} 장착완료");
    }

    public void UnEquip()
    {
        Managers.Data.userData.isWearArray[slotNumber] = false;
        Refresh();
        Managers.Data.SaveUserDataToJson();

        Managers.Game.OnEquipChanged?.Invoke();
        Managers.Game.HpBarOnly?.Invoke();
        Debug.Log($"{Managers.Data.items_Equip[slotNumber].name} 장착해제");


    }



    public void OnButtonEnter(BaseEventData data)
    {
        GetImage((int)Images.Highlight_Back).gameObject.SetActive(true);
        GetImage((int)Images.Highlight_Front).gameObject.SetActive(true);

       uI_Popup_Inven.ShowItemInfo(item_Equip.id);

    }
    public void OnButtonExit(BaseEventData data)
    {
        GetImage((int)Images.Highlight_Back).gameObject.SetActive(false);
        GetImage((int)Images.Highlight_Front).gameObject.SetActive(false);

        uI_Popup_Inven.CloseItemInfo();
    }

}
