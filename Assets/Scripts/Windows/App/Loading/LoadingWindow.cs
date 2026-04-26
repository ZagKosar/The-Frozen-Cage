using DG.Tweening;
using Scripts.WindowSwitcher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Windows.App.Loading
{
    public class LoadingWindow : WindowPanel
    {
        [SerializeField] private Transform _circleLoadingBar;

        private Tween _loadingAnimation;

        public override int Priority => 99;

        public override void Open(object context = null)
        {
            _loadingAnimation = _circleLoadingBar.DORotate(Vector3.forward * -360, 0.8f, RotateMode.FastBeyond360)
                .SetLoops(-1, LoopType.Incremental)
                .SetEase(Ease.Linear);

            gameObject.SetActive(true);
        }

        public override void Close()
        {
            _loadingAnimation.Kill();

            gameObject.SetActive(false);
        }

        public override void Load()
        {
            
        }

        public override void Destroy()
        {
            
        }
    }
}
