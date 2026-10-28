using System.Collections.Generic;

namespace ZeroX.AudioSystem.Editors
{
    public class AudioGroupEditorSettingsCollection
    {
        class Settings
        {
            public bool foldout = false;
            public bool editId = false;
        }

        private Dictionary<long, Settings> dictSettings = new Dictionary<long, Settings>();

        public bool GetFoldout(long groupId)
        {
            if (dictSettings.TryGetValue(groupId, out var settings))
            {
                return settings.foldout;
            }

            return false;
        }

        public void SetFoldout(long groupId, bool foldout)
        {
            if (dictSettings.TryGetValue(groupId, out var settings) == false)
            {
                settings = new Settings();
                dictSettings.Add(groupId, settings);
            }

            settings.foldout = foldout;
        }
        
        public bool GetEditId(long groupId)
        {
            if (dictSettings.TryGetValue(groupId, out var settings))
            {
                return settings.editId;
            }

            return false;
        }

        public void SetEditId(long groupId, bool editId)
        {
            if (dictSettings.TryGetValue(groupId, out var settings) == false)
            {
                settings = new Settings();
                dictSettings.Add(groupId, settings);
            }

            settings.editId = editId;
        }
    }
}