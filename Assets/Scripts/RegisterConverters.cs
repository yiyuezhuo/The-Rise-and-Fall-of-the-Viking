using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Collections.Generic;
using TMPro;
using System.Linq;

using GameCore;



#if UNITY_EDITOR
using UnityEditor;
#endif

public static class RegisteredConverters
{
#if UNITY_EDITOR
    [InitializeOnLoadMethod]
#else
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
#endif
    public static void RegisterConverters()
    {
        Debug.Log("RegisterConverters");


        Register("Bool to DisplayStyle", (ref bool isShow) => (StyleEnum<DisplayStyle>)(isShow ? DisplayStyle.Flex : DisplayStyle.None));
        Register("Bool to DisplayStyle (Not)", (ref bool isShow) => (StyleEnum<DisplayStyle>)(isShow ? DisplayStyle.None : DisplayStyle.Flex));
        Register("object to DisplayStyle", (ref object obj) => (StyleEnum<DisplayStyle>)(obj != null ? DisplayStyle.Flex : DisplayStyle.Flex));
        Register("object to DisplayStyle (Not)", (ref object obj) => (StyleEnum<DisplayStyle>)(obj == null ? DisplayStyle.Flex : DisplayStyle.None));

        // Register("object id to Area name", (ref string id) => EntityManager.current.Get<Area>(id)?.name ?? "[Not specified or invalid]");
    }

    // static ShipClass GetShipClassOfShipLog(NavalCombatCore.ShipLog shipLog)
    // {
    //     return shipLog.shipClass;
    //     // return GameManager.Instance.navalGameState.shipClasses.FirstOrDefault(x => x.name.english == shipLog.shipClassStr);
    // }


    static void Register<TSource, TDestination>(string name, TypeConverter<TSource, TDestination> converter)
    {
        var group = new ConverterGroup(name);
        group.AddConverter(converter);
        ConverterGroups.RegisterConverterGroup(group);
    }
}