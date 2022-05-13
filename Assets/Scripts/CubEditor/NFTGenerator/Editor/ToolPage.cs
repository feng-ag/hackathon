using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace ArcadeGalaxyKit
{
    public class ToolPage : EditorWindow
    {
        static GeneratorTab[] _tabs;
        static string[] _tabNames;
        static ToolPage _instance;
        [MenuItem("Arcade Galaxy Tool Kit/NFT Generator")]
        static void init()
        {
            // Get existing open window or if none, make a new one:
            ToolPage _instance = (ToolPage)GetWindow(typeof(ToolPage));
            _instance.Show();
        }
        static void ResetTabs()
        {
            _tabs = new GeneratorTab[]{
                new BuildTab(),
                new ConfigTab(),
                new PreferenceTab(),
                new VersionLogTab(),
            };
            _tabNames = new string[_tabs.Length];
            for (int c = 0; c < _tabs.Length; c++)
            {
                var names = _tabs[c].GetTabName();
                _tabNames[c] = names;
            }
        }

        void OnEnable()
        {
            if (!Directory.Exists(GeneratorDefaultPath.DefaultDataFolder)) Directory.CreateDirectory(GeneratorDefaultPath.DefaultDataFolder);
            if (!Directory.Exists(GeneratorDefaultPath.DefaultSystemFolder)) Directory.CreateDirectory(GeneratorDefaultPath.DefaultSystemFolder);
            ResetTabs();
            foreach (var tab in _tabs)
            {
                tab.OnEnable();
            }
        }

        int selectedTab = 0;
        void OnGUI()
        {
            if (_tabNames.Length > 0)
            {
                selectedTab = GUILayout.Toolbar(selectedTab, _tabNames);
                if (_tabs[selectedTab].isInit) _tabs[selectedTab].DrawTab();
            }
        }

    }
}