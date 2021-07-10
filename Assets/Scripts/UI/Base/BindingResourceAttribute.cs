using System;
using System.Collections.Generic;
using act.UIRes;

namespace ASeKi.ui
{
    public class BindingResourceAttribute : Attribute
    {
        public UiAssetIndex AssetId { get; private set; }

        public string resourceName = "";

        public BindingResourceAttribute(UiAssetIndex assetId)
        {
            AssetId = assetId;
            resourceName = ResourceName.UINames[(int)AssetId];
        }
    }

    public static class ResourceName
    {
        public static List<string> UINames = new List<string>()
        {
            "UI/Logic/DebugEntry/Prefabs/Main/DebugMainCanvas",
            "UI/Logic/Entry/Prefabs/Main/EntryCanvas"
            // "EntryCanvas"
        };
    }
}
