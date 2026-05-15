using Scripts.Game.Dialog;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;

namespace Scripts.App.ValueProvider
{
    public static class DialogIDProvider
    {
        public static IEnumerable<ValueDropdownItem<string>> GetAllNodeIds()
        {
            var guids = AssetDatabase.FindAssets("t:DialogNode");

            yield return new ValueDropdownItem<string>("", "");

            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);

                var node = AssetDatabase.LoadAssetAtPath<DialogNode>(path);

                if (node == null)
                    continue;

                yield return new ValueDropdownItem<string>(
                    node.name,
                    node.ID);
            }
        }

        public static IEnumerable<ValueDropdownItem<string>> GetAllChoiceIds()
        {
            var guids = AssetDatabase.FindAssets("t:DialogNode");

            yield return new ValueDropdownItem<string>("","");

            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);

                var node = AssetDatabase.LoadAssetAtPath<DialogNode>(path);

                if (node == null)
                    continue;

                foreach (var choice in node.DialogСhoice)
                {
                    yield return new ValueDropdownItem<string>(
                        $"{node.name}/{choice.Text}",
                        choice.ID);
                }
            }
        }
    }
}
