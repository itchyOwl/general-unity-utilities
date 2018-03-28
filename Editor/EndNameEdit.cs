using UnityEditor;
using UnityEditor.ProjectWindowCallback;

namespace ItchyOwl.Editor
{
    internal class EndNameEdit : EndNameEditAction
    {
        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            AssetDatabase.CreateAsset(EditorUtility.InstanceIDToObject(instanceId), AssetDatabase.GenerateUniqueAssetPath(pathName));
        }
    }
}
