using HarmonyLib;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace RefineItemBackgroundColor
{
    [HarmonyPatch]
    public static class Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Trait), nameof(Trait.OnSetCardGrid))]
        public static void OnSetCardGrid_Postfix(Trait __instance, ButtonGrid __0)
        {
            var card = __0.Card;

            // 食べ物アイテムを除外
            if (card.IsFood)
                return;

            // エンチャントが無いアイテムを除外
            if (card.elements == null)
                return;

            // 装備品以外は除く
            if (!card.IsEquipmentOrRanged)
                return;

            // 未鑑定品がレアリティ判明前に背景色が変わってしまう問題を防ぐ
            if (!card.IsIdentified)
                return;

            // ノーマル以下のアイテムは除く
            if (card.rarity is Rarity.Normal or Rarity.Crude or Rarity.Random)
                return;

            var images = __0.GetComponentsInChildren<Image>(true);

            switch (images.Length)
            {
                case 0:
                    return;
                case > 0:
                    {
                        var targetImage = images.FirstOrDefault(image =>
                            image.canvasRenderer.gameObject.name.Contains("ButtonGrid"));
                        if (targetImage != null)
                        {
                            var outline = targetImage.GetComponent<Outline>();

                            if (outline == null)
                                outline = targetImage.gameObject.AddComponent<Outline>();

                            outline.effectColor = Color.white;
                            outline.effectDistance = new Vector2(2, -2);
                            outline.enabled = true;
                        }

                        break;
                    }
            }
        }
    }
};