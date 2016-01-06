﻿using UnityEngine;
using System.Collections;
using Define;

public class AmiscGame
{
    public static string[] arrShutterName = { "shutter_01", "shutter_02", "shutter_03", "shutter_04", "shutter_05", "shutter_06" };

    public static void SetBoxColor(Transform obj, Type type)
    {
        MeshRenderer renderer = obj.GetComponent<MeshRenderer>();
        Color diffuse = GetTypeColorDiffuse(type);
        Color emission = GetTypeColorEmission(type);

        renderer.material.SetColor("_Diffuse", diffuse);
        renderer.material.SetColor("_Emission", emission);
    }

    public static Color GetTypeColorDiffuse(Type type)
    {
        Color color = GetColor(180, 180, 180);

        switch (type)
        {
            case Type.NONE: color = GetColor(180, 180, 180); break;
            case Type.RED: color = GetColor(255, 198, 198); break;
            case Type.BLUE: color = GetColor(142, 202, 194); break;
            case Type.YELLOW: color = GetColor(212, 214, 79); break;
            case Type.GREEN: color = GetColor(188, 234, 219); break;
            case Type.PINK: color = GetColor(202, 144, 143); break;
            case Type.WHITE: color = GetColor(221, 213, 247); break;
            case Type.ORANGE: color = GetColor(214, 177, 141); break;
        }

        return color;
    }

    public static Color GetTypeColorEmission(Type type)
    {
        Color color = GetColor(180, 180, 180);

        switch (type)
        {
            case Type.NONE: color = GetColor(180, 180, 180); break;
            case Type.RED: color = GetColor(193, 97, 97); break;
            case Type.BLUE: color = GetColor(75, 134, 191); break;
            case Type.YELLOW: color = GetColor(168, 171, 118); break;
            case Type.GREEN: color = GetColor(90, 159, 148); break;
            case Type.PINK: color = GetColor(206, 77, 185); break;
            case Type.WHITE: color = GetColor(156, 169, 201); break;
            case Type.ORANGE: color = GetColor(195, 127, 68); break;
        }

        return color;
    }


    //public static void SetColor(Transform obj, Type type)
    //{
    //    MeshRenderer renderer = obj.GetComponent<MeshRenderer>();
    //    Color color = GetTypeColorPastel(type);

    //    renderer.material.color = color;
    //}

    //public static Color GetTypeColorPastel(Type type)
    //{
    //    Color color = GetColor(180, 180, 180);

    //    switch (type)
    //    {
    //        case Type.NONE: color = GetColor(180, 180, 180); break;
    //        case Type.RED: color = GetColor(253, 179, 180); break;
    //        case Type.ORANGE: color = GetColor(253, 215, 179); break;
    //        case Type.YELLOW: color = GetColor(252, 241, 179); break;
    //        case Type.GREEN: color = GetColor(232, 252, 180); break;
    //        case Type.BLUE: color = GetColor(179, 252, 243); break;
    //        case Type.INDIGO: color = GetColor(111, 171, 208); break;
    //        case Type.VIOLET: color = GetColor(193, 188, 246); break;
    //    }

    //    return color;
    //}

    //public static Color GetTypeColor(Type type)
    //{
    //    Color color = GetColor(76, 76, 76);

    //    switch (type)
    //    {
    //        case Type.NONE: color = GetColor(76, 76, 76); break;
    //        case Type.RED: color = GetColor(222, 46, 33); break;
    //        case Type.ORANGE: color = GetColor(247, 143, 30); break;
    //        case Type.YELLOW: color = GetColor(244, 225, 0); break;
    //        case Type.GREEN: color = GetColor(109, 190, 69); break;
    //        case Type.BLUE: color = GetColor(0, 153, 211); break;
    //        case Type.INDIGO: color = GetColor(64, 100, 174); break;
    //        case Type.VIOLET: color = GetColor(145, 116, 180); break;
    //    }

    //    return color;
    //}

    public static Color GetColor(float r, float g, float b, float a = 255)
    {
        return new Color(r / 255.0F, g / 255.0F, b / 255.0F, a / 255.0F);
    }

    public static int GetLevel(int stage)
    {
        int level = 0;

        level = (stage - 1) / 2 + 1;

        return level;
    }
}
