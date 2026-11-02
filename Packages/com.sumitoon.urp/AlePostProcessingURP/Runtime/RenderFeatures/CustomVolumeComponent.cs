using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace SumiToon.PostProcessing
{
    /// <summary>
    /// 后处理容器 基类
    /// </summary>
    public abstract class CustomVolumeComponent : VolumeComponent
    {
        //开启状态
        [HideInInspector]
        public BoolParameter Enable = new BoolParameter(false, false);

        /// <summary>
        /// 是否已经强制打开过 Enable 的 overrideState。
        /// OnEnable 会在 Volume 栈重建时被频繁调用，
        /// 若每次都写 overrideState，用户在 Inspector 里永远无法取消该 override。
        /// 因此只在实例首次启用时设置一次。
        /// </summary>
        private bool m_EnableOverrideInitialized;

        protected override void OnEnable()
        {
            base.OnEnable();

            //VolumeComponent.active 有Bug始终为true
            //使用Enable的overrideState作为Volume的激活状态
            if (m_EnableOverrideInitialized) return;

            Enable.overrideState = true;
            m_EnableOverrideInitialized = true;
        }
    }
}