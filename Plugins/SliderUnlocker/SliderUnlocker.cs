﻿using BepInEx;
using ChaCustom;
using Harmony;
using Illusion.Component.UI.ColorPicker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SliderUnlocker
{
    public class SliderUnlocker : BaseUnityPlugin
    {
        public override string Name => "Slider Unlocker";

        public static float Minimum = -1.0f;
        public static float Maximum = 2.0f;


        private static FieldInfo akf_sliderR = (typeof(PickerSlider).GetField("sliderR", BindingFlags.NonPublic | BindingFlags.Instance));
        private static FieldInfo akf_sliderG = (typeof(PickerSlider).GetField("sliderG", BindingFlags.NonPublic | BindingFlags.Instance));
        private static FieldInfo akf_sliderB = (typeof(PickerSlider).GetField("sliderB", BindingFlags.NonPublic | BindingFlags.Instance));
        private static FieldInfo akf_sliderA = (typeof(PickerSlider).GetField("sliderA", BindingFlags.NonPublic | BindingFlags.Instance));

        public SliderUnlocker()
        {
            var harmony = HarmonyInstance.Create("com.bepis.bepinex.sliderunlocker");

            MethodInfo original = AccessTools.Method(typeof(CustomBase), "ConvertTextFromRate");

            HarmonyMethod postfix = new HarmonyMethod(typeof(Hooks).GetMethod("ConvertTextFromRateHook"));
            
            harmony.Patch(original, null, postfix);



            original = AccessTools.Method(typeof(CustomBase), "ConvertRateFromText");

            postfix = new HarmonyMethod(typeof(Hooks).GetMethod("ConvertRateFromTextHook"));

            harmony.Patch(original, null, postfix);



            original = AccessTools.Method(typeof(Mathf), "Clamp", new Type[] { typeof(float), typeof(float), typeof(float) });
            
            postfix = new HarmonyMethod(typeof(Hooks).GetMethod("MathfClampHook"));

            harmony.Patch(original, null, postfix);




            original = typeof(AnimationKeyInfo).GetMethods().Where(x => x.Name.Contains("GetInfo")).ToArray()[0];

            var prefix = new HarmonyMethod(typeof(Hooks).GetMethod("GetInfoSingularPreHook"));

            postfix = new HarmonyMethod(typeof(Hooks).GetMethod("GetInfoSingularPostHook"));

            harmony.Patch(original, prefix, postfix);



            original = typeof(AnimationKeyInfo).GetMethods().Where(x => x.Name.Contains("GetInfo")).ToArray()[1];
            
            prefix = new HarmonyMethod(typeof(Hooks).GetMethod("GetInfoPreHook"));

            postfix = new HarmonyMethod(typeof(Hooks).GetMethod("GetInfoPostHook"));

            harmony.Patch(original, prefix, postfix);
        }

        protected override void LevelFinishedLoading(Scene scene, LoadSceneMode mode)
        {
            foreach (Slider gameObject in GameObject.FindObjectsOfType<Slider>())
            {
                gameObject.minValue = Minimum;
                gameObject.maxValue = Maximum;
            }

            foreach (GameObject gameObject in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                if (gameObject.name.ToUpper().StartsWith("CVS"))
                {
                    Console.WriteLine(gameObject.name);
                }
            }

            foreach (GameObject gameObject in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                if (gameObject.name == "PickerSliderColor" ||
                    gameObject.name == "menuSlider")
                {
                    foreach (Slider slider in gameObject.GetComponents<Slider>())
                    {
                        slider.maxValue = 1f;
                    }
                }
            }

            foreach (PickerSlider gameObject in GameObject.FindObjectsOfType<PickerSlider>())
            {
                ((Slider)akf_sliderA.GetValue(gameObject)).maxValue = 1f;
                ((Slider)akf_sliderR.GetValue(gameObject)).maxValue = 1f;
                ((Slider)akf_sliderG.GetValue(gameObject)).maxValue = 1f;
                ((Slider)akf_sliderB.GetValue(gameObject)).maxValue = 1f;
            }
        }
    }
}