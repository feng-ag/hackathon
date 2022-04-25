using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBase : MonoBehaviour
{

    public int itemType;
    public Vector2 pos;
    public float Rotate => inner.rotation.y;

    public ItemData.Item Item => MapEditorManager.Instance.itemData.GetItemAt(itemType);
    public Transform inner;


    public void RotateLeft()
    {
        inner.Rotate(0, 90, 0);
        SyncData();
    }

    public void RotateRight()
    {
        inner.Rotate(0, -90, 0);
        SyncData();
    }

    void SyncData()
    {
        MapEditorManager.MapEditorItemData mapEditorItemData = MapEditorManager.Instance.mapEditorItemDataQuery[pos];

        mapEditorItemData.rotate = inner.localRotation.eulerAngles.y;

        MapEditorManager.Instance.mapEditorItemDataQuery[pos] = mapEditorItemData;
    }

}
