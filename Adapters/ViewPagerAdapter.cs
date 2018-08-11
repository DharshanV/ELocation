using Java.Lang;
using MyFragment = Android.Support.V4.App.Fragment;
using MyFragmentManager = Android.Support.V4.App.FragmentManager;

namespace ELocation
{
    class ViewPagerAdapter : Android.Support.V4.App.FragmentPagerAdapter
    {
        public ViewPagerAdapter(MyFragmentManager fm) : base(fm) { }

        public override MyFragment GetItem(int position)
        {
            MyFragment fragment = new MyFragment();
            switch (position)
            {
                case 0:
                    fragment = new HomeFragment();
                    break;
                case 1:
                    fragment = new MapFragment();
                    break;
            }
            return fragment;
        }

        public override int Count { get { return 2; } }

        public override ICharSequence GetPageTitleFormatted(int position)
        {
            ICharSequence title = null;
            switch (position)
            {
                case 0:
                    title = new Java.Lang.String("Home");
                    break;
                case 1:
                    title = new Java.Lang.String("Map");
                    break;
            }
            return title;
        }
    }
}