using System.Linq;
using System.Windows.Forms;

namespace AbsoluteAlexander_MaliktenderOnly
{
    public class TabPageManager
    {
        private class TabPageInfo
        {
            public TabPage TabPage;
            public bool Visible;
            public TabPageInfo(TabPage page, bool v)
            {
                TabPage = page;
                Visible = v;
            }
        }
        private TabPageInfo[] _tabPageInfos = null;
        private TabControl _tabControl = null;

        /// <summary>
        /// TabPageManagerクラスのインスタンスを作成する
        /// </summary>
        /// <param name="crl">基になるTabControlオブジェクト</param>
        public TabPageManager(TabControl crl)
        {
            _tabControl = crl;
            _tabPageInfos = new TabPageInfo[_tabControl.TabPages.Count];
            for (int i = 0; i < _tabControl.TabPages.Count; i++)
                _tabPageInfos[i] =
                    new TabPageInfo(_tabControl.TabPages[i], true);
        }

        /// <summary>
        /// TabPageの表示・非表示を変更する
        /// </summary>
        /// <param name="index">変更するTabPageのIndex番号</param>
        /// <param name="v">表示するときはTrue。
        /// 非表示にするときはFalse。</param>
        public void ChangeTabPageVisible(int index, bool v)
        {
            if (_tabPageInfos[index].Visible == v)
                return;

            _tabPageInfos[index].Visible = v;
            _tabControl.SuspendLayout();
            _tabControl.TabPages.Clear();
            for (int i = 0; i < _tabPageInfos.Length; i++)
            {
                if (_tabPageInfos[i].Visible)
                    _tabControl.TabPages.Add(_tabPageInfos[i].TabPage);
            }
            _tabControl.ResumeLayout();
        }
    }
}
