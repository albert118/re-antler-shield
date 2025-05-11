using BepInEx;
using R2API;
using R2API.Utils;
using RoR2;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace ExamplePlugin;

[BepInDependency(ItemAPI.PluginGUID)]
[BepInDependency(LanguageAPI.PluginGUID)]
[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
[NetworkCompatibility]
public class ReAntlerShield : BaseUnityPlugin
{
    public const string PluginGuid = $"com.{PluginAuthor}.{PluginName}";
    public const string PluginAuthor = "albert118";
    public const string PluginName = "ReAntlerShield";
    public const string PluginVersion = "1.0.0";

    private static ItemDef _itemDef;

    public void Awake()
    {
        CreateItem();
        GameModeCatalog.availability.CallWhenAvailable(PostLoad);
    }

    public void CreateItem()
    {
        _itemDef = ScriptableObject.CreateInstance<ItemDef>();

        LanguageFileLoader.AddLanguageFilesFromMod(this, "languages");

        _itemDef.name = "NEGATEATTACK_NAME";
        _itemDef.nameToken = "NEGATEATTACK_NAME";
        _itemDef.pickupToken = "NEGATEATTACK_PICKUP";
        _itemDef.descriptionToken = "NEGATEATTACK_DESC";
        _itemDef.loreToken = "NEGATEATTACK_LORE";

        _itemDef._itemTierDef = Addressables
            .LoadAssetAsync<ItemTierDef>("RoR2/Base/Common/Tier1Def.asset")
            .WaitForCompletion();

        _itemDef.canRemove = true;
        _itemDef.canRemove = false;

        ItemAPI.Add(new CustomItem(_itemDef, new ItemDisplayRuleDict(null)));
    }

    public void PostLoad()
    {
        UpdateItemDef();
    }

    private void UpdateItemDef()
    {
        // use Elusive Antlers to replace icons, etc.
        // NegateAttack asset aka. "Antler Shield"
        // see https://xiaoxiao921.github.io/GithubActionCacheTest/assetPathsDump.html
        // antler shield is no longer an asset, so we load the new "Elusive Antlers" asset instead
        // this always throws an invalid key exception looking it up - not sure why as it's a listed asset
        var elusiveAntlerItemDef = Addressables
            .LoadAssetAsync<ItemDef>("RoR2/DLC2/Items/SpeedBoostPickup/Items.ElusiveAntlers.asset")
            .WaitForCompletion();

        if (elusiveAntlerItemDef == null) {
            Logger.LogError("Failed to load the Elusive Antlers asset - bailing out");
            return;
        }

        _itemDef.pickupIconSprite = elusiveAntlerItemDef.pickupIconSprite;
        _itemDef.pickupModelPrefab = elusiveAntlerItemDef.pickupModelPrefab;
    }
}
