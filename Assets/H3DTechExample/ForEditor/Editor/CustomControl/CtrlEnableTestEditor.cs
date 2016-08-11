using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class CtrlEnableTestEditor
{
    static EditorRoot m_EditorRoot = null;                        //根控件

    [UnityEditor.MenuItem("Assets/H3D/控件测试工具/控件置灰测试1")]
    [UnityEditor.MenuItem("H3D/UI/控件测试工具/控件置灰测试1")]

    static void Init()
    {
        EditorRoot root = EditorManager.GetInstance().FindEditor("控件置灰测试1");
        if (root == null)
        {
            EditorManager.GetInstance().RemoveEditor("控件置灰测试1");
            root = EditorManager.GetInstance().CreateEditor("控件置灰测试1", false, InitControls);
        }
    }

    public static void InitControls(EditorRoot editorRoot)
    {
        if (editorRoot == null)
        {
            //提示程序错误Message
            EditorUtility.DisplayDialog("运行错误",
                                         "窗口初始化失败",
                                         "确认");
            return;
        }

        m_EditorRoot = editorRoot;
        //m_EditorRoot.position = new Rect(100f, 100f, 1920f, 1080f);

        //Rect sliderRect = new Rect(0, 0, 300, 20);
        //Rect textRect = new Rect(0, 0, 300, 20);
        //Rect filedRect = new Rect(0, 0, 300, 20);
        //Rect boundsRect = new Rect(0, 0, 200, 80);
        //Rect labelRect = new Rect(0, 0, 200, 80);
        //Rect btnRect = new Rect(0, 0, 100, 20);
        Rect listRect = new Rect(0, 0, 200, 80);

        string[] MFStrings = new string[] { "CanJump", "CanShoot", "CanSwim" };
        string[] selStrings = new string[] { "Grid 1", "Grid 2", "Grid 3", "Grid 4" };
        string[] toolBarStrings = new string[] { "ToolBar 1", "ToolBar 2", "ToolBar 3", "ToolBar 4" };

        StaticEditorFlags item = StaticEditorFlags.BatchingStatic;

        HSpliterCtrl hs1 = new HSpliterCtrl();
        hs1.layoutConstraint = LayoutConstraint.GetSpliterConstraint(80f);

        HSpliterCtrl hs2 = new HSpliterCtrl();
        hs2.layoutConstraint = LayoutConstraint.GetSpliterConstraint(80f);

        HSpliterCtrl hs3 = new HSpliterCtrl();
        hs3.layoutConstraint = LayoutConstraint.GetSpliterConstraint(180f);

        HSpliterCtrl hs4 = new HSpliterCtrl();
        hs4.layoutConstraint = LayoutConstraint.GetSpliterConstraint(80f);

        HSpliterCtrl hs5 = new HSpliterCtrl();
        hs5.layoutConstraint = LayoutConstraint.GetSpliterConstraint(80f);

        HSpliterCtrl hs6 = new HSpliterCtrl();
        hs6.layoutConstraint = LayoutConstraint.GetSpliterConstraint(80f);

        HSpliterCtrl hs7 = new HSpliterCtrl();
        hs7.layoutConstraint = LayoutConstraint.GetSpliterConstraint(200f);

        HSpliterCtrl hs8 = new HSpliterCtrl();
        hs8.layoutConstraint = LayoutConstraint.GetSpliterConstraint(80f);


        HBoxCtrl hb1 = new HBoxCtrl(true);
        HBoxCtrl hb2 = new HBoxCtrl(true);
        HBoxCtrl hb3 = new HBoxCtrl(true);
        HBoxCtrl hb4 = new HBoxCtrl(true);
        HBoxCtrl hb5 = new HBoxCtrl(true);
        HBoxCtrl hb6 = new HBoxCtrl(true);
        VBoxCtrl vb7 = new VBoxCtrl(true);
        vb7.onDoubleClick = OnVboxDoubleClick;
        vb7.onClick = OnVboxClick;
        vb7.onOnPress = OnVboxPress;

        HBoxCtrl hb8 = new HBoxCtrl(true);

        hb1.onDoubleClick = OnHboxDoubleClick;
        hb1.onClick = OnHboxClick;
        hb1.onOnPress = OnHboxPress;

        hb2.onDoubleClick = OnHboxDoubleClick;
        hb2.onClick = OnHboxClick;
        hb2.onOnPress = OnHboxPress;

        hb3.onDoubleClick = OnHboxDoubleClick;
        hb3.onClick = OnHboxClick;
        hb3.onOnPress = OnHboxPress;

        hb4.onDoubleClick = OnHboxDoubleClick;
        hb4.onClick = OnHboxClick;
        hb4.onOnPress = OnHboxPress;

        hb5.onDoubleClick = OnHboxDoubleClick;
        hb5.onClick = OnHboxClick;
        hb5.onOnPress = OnHboxPress;

        hb6.onDoubleClick = OnHboxDoubleClick;
        hb6.onClick = OnHboxClick;
        hb6.onOnPress = OnHboxPress;

        hb8.onDoubleClick = OnHboxDoubleClick;
        hb8.onClick = OnHboxClick;
        hb8.onOnPress = OnHboxPress;

        HBoxCtrl hb9 = new HBoxCtrl(true);

        #region hb1
        DataFieldCtrl<Bounds> boundCtrl = new DataFieldCtrl<Bounds>();
        boundCtrl.Name = "_BoundCtrl";
        boundCtrl.Caption = "1.Bounds:";

        ButtonCtrl boundCtrlEnableBtn = new ButtonCtrl();
        boundCtrlEnableBtn.Name = "_BoundCtrlEnableBtn";
        boundCtrlEnableBtn.Caption = "无效";
        boundCtrlEnableBtn.UserDefData = boundCtrl;
        boundCtrlEnableBtn.onClick = OnEnableBtnClick;

        ButtonCtrl testBtn = new ButtonCtrl();
        testBtn.Name = "_TestBtn";
        testBtn.Caption = "2.TestBtn";
        testBtn.onClick = OnTestBtnClick;

        ButtonCtrl testBtnEnableBtn = new ButtonCtrl();
        testBtnEnableBtn.Name = "_TestBtnEnableBtn";
        testBtnEnableBtn.Caption = "无效";
        testBtnEnableBtn.onClick = OnEnableBtnClick;
        testBtnEnableBtn.UserDefData = testBtn;

        ColorCtrl colorCtrl = new ColorCtrl();
        colorCtrl.Name = "_ColorName";
        colorCtrl.Caption = "3.Color:";

        ButtonCtrl colorCtrlEnableBtn = new ButtonCtrl();
        colorCtrlEnableBtn.Name = "_ColorCtrlEnableBtn";
        colorCtrlEnableBtn.Caption = "无效";
        colorCtrlEnableBtn.UserDefData = colorCtrl;
        colorCtrlEnableBtn.onClick = OnEnableBtnClick;

        ComboBoxCtrl<int> comboboxCtrl = new ComboBoxCtrl<int>();
        comboboxCtrl.Name = "_ComboboxCtrl";
        comboboxCtrl.Caption = "4.Combo";
        comboboxCtrl.AddItem(new ComboItem("test1", 0));
        comboboxCtrl.AddItem(new ComboItem("test2", 1));

        ButtonCtrl comboboxCtrlEnable = new ButtonCtrl();
        comboboxCtrlEnable.Name = "_ColorCtrlEnableBtn";
        comboboxCtrlEnable.Caption = "无效";
        comboboxCtrlEnable.UserDefData = comboboxCtrl;
        comboboxCtrlEnable.onClick = OnEnableBtnClick;

        CurveEditorCtrl curveCtrl = new CurveEditorCtrl();
        curveCtrl.Name = "_CurveCtrl";
        curveCtrl.Caption = "5.Curve";

        ButtonCtrl curveCtrlEnableBtn = new ButtonCtrl();
        curveCtrlEnableBtn.Name = "_ColorCtrlEnableBtn";
        curveCtrlEnableBtn.Caption = "无效";
        curveCtrlEnableBtn.UserDefData = curveCtrl;
        curveCtrlEnableBtn.onClick = OnEnableBtnClick;

        ComboBoxCtrl<Enum> enumComboBox = new ComboBoxCtrl<Enum>(item);
        enumComboBox.Name = "_EnumComboBox";
        enumComboBox.Caption = "6.Combo";

        ButtonCtrl enumComboEnableBtn = new ButtonCtrl();
        enumComboEnableBtn.Name = "_EnumComboEnableBtn";
        enumComboEnableBtn.Caption = "无效";
        enumComboEnableBtn.UserDefData = enumComboBox;
        enumComboEnableBtn.onClick = OnEnableBtnClick;
        #endregion

        #region hb2

        MaskFieldCtrl<Enum> enumMaskField = new MaskFieldCtrl<Enum>(item);
        enumMaskField.Caption = "7.EnumMask:";
        enumMaskField.Name = "_EnumMaskField";

        ButtonCtrl enumMaskEnableBtn = new ButtonCtrl();
        enumMaskEnableBtn.Name = "_EnumComboEnableBtn";
        enumMaskEnableBtn.Caption = "无效";
        enumMaskEnableBtn.UserDefData = enumMaskField;
        enumMaskEnableBtn.onClick = OnEnableBtnClick;

        DataFieldCtrl<float> floatFiled = new DataFieldCtrl<float>();
        floatFiled.Name = "_FloatField";
        floatFiled.Caption = "8.FloatField:";

        ButtonCtrl floatFiledkEnableBtn = new ButtonCtrl();
        floatFiledkEnableBtn.Name = "_FloatFiledkEnableBtn";
        floatFiledkEnableBtn.Caption = "无效";
        floatFiledkEnableBtn.UserDefData = floatFiled;
        floatFiledkEnableBtn.onClick = OnEnableBtnClick;

        SliderCtrl<float> floatSlider = new SliderCtrl<float>(0);
        floatSlider.Name = "_FloatSlider";
        floatSlider.Caption = "9.FloatSlider:";
        floatSlider.ValueRange = new Vector2(0, 20);

        ButtonCtrl floatSliderEnableBtn = new ButtonCtrl();
        floatSliderEnableBtn.Name = "_FloatSliderEnableBtn";
        floatSliderEnableBtn.Caption = "无效";
        floatSliderEnableBtn.UserDefData = floatSlider;
        floatSliderEnableBtn.onClick = OnEnableBtnClick;

        DataFieldCtrl<int> intField = new DataFieldCtrl<int>();
        intField.Name = "_IntField";
        intField.Caption = "10.IntField:";

        ButtonCtrl intFieldEnableBtn = new ButtonCtrl();
        intFieldEnableBtn.Name = "_IntFieldEnableBtn";
        intFieldEnableBtn.Caption = "无效";
        intFieldEnableBtn.UserDefData = intField;
        intFieldEnableBtn.onClick = OnEnableBtnClick;

        SliderCtrl<int> intSlider = new SliderCtrl<int>(0);
        intSlider.Name = "_IntSlider";
        intSlider.Caption = "11.IntSlider:";
        intSlider.ValueRange = new Vector2(0, 20);

        ButtonCtrl intSliderEnableBtn = new ButtonCtrl();
        intSliderEnableBtn.Name = "_IntSliderEnableBtn";
        intSliderEnableBtn.Caption = "无效";
        intSliderEnableBtn.UserDefData = intSlider;
        intSliderEnableBtn.onClick = OnEnableBtnClick;
        #endregion

        #region hb3
        LayerFieldCtrl layerField = new LayerFieldCtrl();
        layerField.Name = "_LayerField";
        layerField.Caption = "12.LayerField:";

        ButtonCtrl layerFieldEnableBtn = new ButtonCtrl();
        layerFieldEnableBtn.Name = "_layerFieldEnableBtn";
        layerFieldEnableBtn.Caption = "无效";
        layerFieldEnableBtn.UserDefData = layerField;
        layerFieldEnableBtn.onClick = OnEnableBtnClick;

        ListViewCtrl listCtrl = new ListViewCtrl();
        listCtrl.Name = "_ListCtrl";
        listCtrl.Size = listRect;
        listCtrl.onDoubleClick = OnListDoubleClick;
        listCtrl.onOnPress = OnListOnPressClick;

        ButtonCtrl listCtrlEnableBtn = new ButtonCtrl();
        listCtrlEnableBtn.Name = "_ListCtrlEnableBtn";
        listCtrlEnableBtn.Caption = "无效";
        listCtrlEnableBtn.UserDefData = listCtrl;
        listCtrlEnableBtn.onClick = OnEnableBtnClick;

        MaskFieldCtrl<int> maskField = new MaskFieldCtrl<int>(0);
        maskField.Caption = "14.MaskField:";
        maskField.Name = "_MaskField";
        maskField.AddItem(MFStrings.ToList<string>());

        ButtonCtrl maskFieldEnableBtn = new ButtonCtrl();
        maskFieldEnableBtn.Name = "_MaskFieldEnableBtn";
        maskFieldEnableBtn.Caption = "无效";
        maskFieldEnableBtn.UserDefData = maskField;
        maskFieldEnableBtn.onClick = OnEnableBtnClick;

        SliderCtrl<Vector2> mimmaxSlider = new SliderCtrl<Vector2>();
        mimmaxSlider.Name = "_MinMaxSlider";
        mimmaxSlider.Caption = "15.MinMax值:";
        mimmaxSlider.ValueRange = new Vector2(-20, 20);

        ButtonCtrl mimmaxSliderEnableBtn = new ButtonCtrl();
        mimmaxSliderEnableBtn.Name = "_MimmaxSliderEnableBtn";
        mimmaxSliderEnableBtn.Caption = "无效";
        mimmaxSliderEnableBtn.UserDefData = mimmaxSlider;
        mimmaxSliderEnableBtn.onClick = OnEnableBtnClick;

        ObjectFieldCtrl objectField = new ObjectFieldCtrl();
        objectField.Name = "_ObjectField";
        objectField.Caption = "16.Object:";
        objectField.ObjectType = typeof(object);

        ButtonCtrl objectFieldEnableBtn = new ButtonCtrl();
        objectFieldEnableBtn.Name = "_ObjectFieldEnableBtn";
        objectFieldEnableBtn.Caption = "无效";
        objectFieldEnableBtn.UserDefData = objectField;
        objectFieldEnableBtn.onClick = OnEnableBtnClick;


        PasswordFieldCtrl passwordField = new PasswordFieldCtrl();
        passwordField.Name = "_PasswordField";
        passwordField.Caption = "17.Password:";

        ButtonCtrl passwordFieldEnableBtn = new ButtonCtrl();
        passwordFieldEnableBtn.Name = "_PasswordFieldEnableBtn";
        passwordFieldEnableBtn.Caption = "无效";
        passwordFieldEnableBtn.UserDefData = passwordField;
        passwordFieldEnableBtn.onClick = OnEnableBtnClick;
        #endregion

        #region hb4
        DataFieldCtrl<Rect> rectField = new DataFieldCtrl<Rect>(new Rect(0, 0, 0, 0));
        rectField.Name = "_RectField";
        rectField.Caption = "18.Rect:";

        ButtonCtrl rectFieldEnableBtn = new ButtonCtrl();
        rectFieldEnableBtn.Name = "_RectFieldEnableBtn";
        rectFieldEnableBtn.Caption = "无效";
        rectFieldEnableBtn.UserDefData = rectField;
        rectFieldEnableBtn.onClick = OnEnableBtnClick;

        SelectionGridCtrl selectionGrid = new SelectionGridCtrl();
        selectionGrid.Caption = "19.SelectionGrid:";
        selectionGrid.Name = "_SelectionGrid";
        selectionGrid.XCount = 4;
        selectionGrid.AddItem(selStrings.ToList<string>());

        ButtonCtrl selectionGridEnableBtn = new ButtonCtrl();
        selectionGridEnableBtn.Name = "_SelectionGridEnableBtn";
        selectionGridEnableBtn.Caption = "无效";
        selectionGridEnableBtn.UserDefData = selectionGrid;
        selectionGridEnableBtn.onClick = OnEnableBtnClick;

        TagFieldCtrl tagField = new TagFieldCtrl();
        tagField.Name = "_TagField";
        tagField.Caption = "20.Tag:";

        ButtonCtrl tagFieldEnableBtn = new ButtonCtrl();
        tagFieldEnableBtn.Name = "_TagFieldEnableBtn";
        tagFieldEnableBtn.Caption = "无效";
        tagFieldEnableBtn.UserDefData = tagField;
        tagFieldEnableBtn.onClick = OnEnableBtnClick;

        TextAreaCtrl textArea = new TextAreaCtrl();
        textArea.Name = "_TextArea";
        textArea.Caption = "21.TextArea:";

        ButtonCtrl textAreaEnableBtn = new ButtonCtrl();
        textAreaEnableBtn.Name = "_TextAreaEnableBtn";
        textAreaEnableBtn.Caption = "无效";
        textAreaEnableBtn.UserDefData = textArea;
        textAreaEnableBtn.onClick = OnEnableBtnClick;

        TextBoxCtrl textBox = new TextBoxCtrl();
        textBox.Name = "_TextBox";
        textBox.Text = "AAA";
        textBox.Caption = "22.Text:";

        ButtonCtrl textBoxEnableBtn = new ButtonCtrl();
        textBoxEnableBtn.Name = "_TextBoxEnableBtn";
        textBoxEnableBtn.Caption = "无效";
        textBoxEnableBtn.UserDefData = textBox;
        textBoxEnableBtn.onClick = OnEnableBtnClick;

        #endregion

        #region hb5
        ToggleCtrl toggleCtrl = new ToggleCtrl();
        toggleCtrl.Name = "_ToggleCtrl";
        toggleCtrl.Caption = "23.Toggle";

        ButtonCtrl toggleCtrlEnableBtn = new ButtonCtrl();
        toggleCtrlEnableBtn.Name = "_ToggleCtrlEnableBtn";
        toggleCtrlEnableBtn.Caption = "无效";
        toggleCtrlEnableBtn.UserDefData = toggleCtrl;
        toggleCtrlEnableBtn.onClick = OnEnableBtnClick;

        ToolBarCtrl toolBar = new ToolBarCtrl();
        toolBar.Name = "_ToolBar";
        toolBar.Caption = "24.ToolBar:";
        toolBar.AddItem(toolBarStrings.ToList<string>());

        ButtonCtrl toolBarEnableBtn = new ButtonCtrl();
        toolBarEnableBtn.Name = "_ToolBarEnableBtn";
        toolBarEnableBtn.Caption = "无效";
        toolBarEnableBtn.UserDefData = toolBar;
        toolBarEnableBtn.onClick = OnEnableBtnClick;

        DataFieldCtrl<Vector2> vector2Field = new DataFieldCtrl<Vector2>(new Vector2(0, 0));
        vector2Field.Name = "_Vector2Field";
        vector2Field.Caption = "25.Vecetor2值:";

        ButtonCtrl vector2FieldEnableBtn = new ButtonCtrl();
        vector2FieldEnableBtn.Name = "_Vector2FieldEnableBtn";
        vector2FieldEnableBtn.Caption = "无效";
        vector2FieldEnableBtn.UserDefData = vector2Field;
        vector2FieldEnableBtn.onClick = OnEnableBtnClick;

        DataFieldCtrl<Vector3> vector3Field = new DataFieldCtrl<Vector3>(new Vector3(0, 0, 0));
        vector3Field.Name = "_Vector3Field";
        vector3Field.Caption = "26.Vecetor3值:";

        ButtonCtrl vector3FieldEnableBtn = new ButtonCtrl();
        vector3FieldEnableBtn.Name = "_Vector3FieldEnableBtn";
        vector3FieldEnableBtn.Caption = "无效";
        vector3FieldEnableBtn.UserDefData = vector3Field;
        vector3FieldEnableBtn.onClick = OnEnableBtnClick;

        DataFieldCtrl<Vector4> vector4Field = new DataFieldCtrl<Vector4>(new Vector4(0, 0, 0, 0));
        vector4Field.Name = "_Vector4Field";
        vector4Field.Caption = "27.Vecetor4值:";

        ButtonCtrl vector4FieldEnableBtn = new ButtonCtrl();
        vector4FieldEnableBtn.Name = "_Vector4FieldEnableBtn";
        vector4FieldEnableBtn.Caption = "无效";
        vector4FieldEnableBtn.UserDefData = vector4Field;
        vector4FieldEnableBtn.onClick = OnEnableBtnClick;
        #endregion

        #region hb6
        PlayCtrl playCtrl = new PlayCtrl();
        playCtrl.Name = "_PlayCtrl";
        playCtrl.TotalTime = 10f;

        ButtonCtrl playCtrlEnableBtn = new ButtonCtrl();
        playCtrlEnableBtn.Name = "_PlayCtrlEnableBtn";
        playCtrlEnableBtn.Caption = "无效";
        playCtrlEnableBtn.UserDefData = playCtrl;
        playCtrlEnableBtn.onClick = OnEnableBtnClick;
        #endregion

        #region vb7
        TimeLineViewCtrl timeLineCtrl = new TimeLineViewCtrl();
        timeLineCtrl.TotalTime = 10;

        timeLineCtrl.Tags.Add(new TimeTag());
        TimeLineItem timeLineItem = new TimeLineItem();
        timeLineItem.startTime = 0;
        timeLineItem.length = 0;
        timeLineItem.loop = false;
        timeLineCtrl.Items.Add(timeLineItem);

        ButtonCtrl timeLineCtrlEnableBtn = new ButtonCtrl();
        timeLineCtrlEnableBtn.Name = "_TimeLineCtrlEnableBtn";
        timeLineCtrlEnableBtn.Caption = "无效";
        timeLineCtrlEnableBtn.UserDefData = timeLineCtrl;
        timeLineCtrlEnableBtn.onClick = OnEnableBtnClick;

        #endregion

        #region hb8
        ButtonCtrl hb5EnableBtn = new ButtonCtrl();
        hb5EnableBtn.Name = "_Hb5EnableBtn";
        hb5EnableBtn.Caption = "无效hb5";
        hb5EnableBtn.UserDefData = hb5;
        hb5EnableBtn.onClick = OnEnableBtnClick;

        ButtonCtrl vb7EnableBtn = new ButtonCtrl();
        vb7EnableBtn.Name = "_Vb7EnableBtn";
        vb7EnableBtn.Caption = "无效vb7";
        vb7EnableBtn.UserDefData = vb7;
        vb7EnableBtn.onClick = OnEnableBtnClick;

        ButtonCtrl changeV2ValueBtn = new ButtonCtrl();
        changeV2ValueBtn.Name = "_ChangeValueBtn";
        changeV2ValueBtn.Caption = "ChangeValue V2";
        changeV2ValueBtn.Size = new Rect(0, 0, 120, 20);
        changeV2ValueBtn.onClick = OnChangeValueBtnClick;

        ButtonCtrl changeIntValueBtn = new ButtonCtrl();
        changeIntValueBtn.Name = "_ChangeIntValueBtn";
        changeIntValueBtn.Caption = "ChangeValue Int";
        changeIntValueBtn.Size = new Rect(0, 0, 120, 20);
        changeIntValueBtn.onClick = OnChangeIntValueBtnClick;

        #endregion

        #region hb9
        LabelCtrl labelCtrl = new LabelCtrl();
        labelCtrl.Name = "_LableCtrl";
        labelCtrl.Caption = "Just A Label";
        labelCtrl.onDoubleClick = OnLabelDoubleClick;
        labelCtrl.onClick = OnLabelClick;
        labelCtrl.onOnPress = OnLabelPress;

        ButtonCtrl repeatButton = new ButtonCtrl();
        repeatButton.Caption = "Repeat";
        repeatButton.IsRepeat = true;
        repeatButton.onClick = OnRepeatButtonPress;

        #endregion
        hs1.Add(hb1);
        hs1.Add(hs2);

        hs2.Add(hb2);
        hs2.Add(hs3);

        hs3.Add(hb3);
        hs3.Add(hs4);

        hs4.Add(hb4);
        hs4.Add(hs5);

        hs5.Add(hb5);
        hs5.Add(hs6);

        hs6.Add(hb6);
        hs6.Add(hs7);

        hs7.Add(vb7);
        hs7.Add(hs8);

        hs8.Add(hb8);
        hs8.Add(hb9);
        #region AddHb1
        hb1.Add(boundCtrl);
        hb1.Add(boundCtrlEnableBtn);

        SpaceCtrl space1_1 = new SpaceCtrl();
        space1_1.CurrValue = 20f;
        hb1.Add(space1_1);

        hb1.Add(testBtn);
        hb1.Add(testBtnEnableBtn);

        SpaceCtrl space1_2 = new SpaceCtrl();
        space1_2.CurrValue = 20f;
        hb1.Add(space1_2);

        hb1.Add(colorCtrl);
        hb1.Add(colorCtrlEnableBtn);

        SpaceCtrl space1_3 = new SpaceCtrl();
        space1_3.CurrValue = 20f;
        hb1.Add(space1_3);

        hb1.Add(comboboxCtrl);
        hb1.Add(comboboxCtrlEnable);

        SpaceCtrl space1_4 = new SpaceCtrl();
        space1_4.CurrValue = 20f;
        hb1.Add(space1_4);

        hb1.Add(curveCtrl);
        hb1.Add(curveCtrlEnableBtn);

        SpaceCtrl space1_5 = new SpaceCtrl();
        space1_5.CurrValue = 20f;
        hb1.Add(space1_5);

        hb1.Add(enumComboBox);
        hb1.Add(enumComboEnableBtn);
        #endregion

        #region AddHb2
        hb2.Add(enumMaskField);
        hb2.Add(enumMaskEnableBtn);

        SpaceCtrl space2_1 = new SpaceCtrl();
        space2_1.CurrValue = 20f;
        hb2.Add(space2_1);

        hb2.Add(floatFiled);
        hb2.Add(floatFiledkEnableBtn);

        SpaceCtrl space2_2 = new SpaceCtrl();
        space2_2.CurrValue = 20f;
        hb2.Add(space2_2);

        hb2.Add(floatSlider);
        hb2.Add(floatSliderEnableBtn);

        SpaceCtrl space2_3 = new SpaceCtrl();
        space2_3.CurrValue = 20f;
        hb2.Add(space2_3);

        hb2.Add(intField);
        hb2.Add(intFieldEnableBtn);

        SpaceCtrl space2_4 = new SpaceCtrl();
        space2_4.CurrValue = 20f;
        hb2.Add(space2_4);

        hb2.Add(intSlider);
        hb2.Add(intSliderEnableBtn);
        #endregion

        #region AddHb3
        hb3.Add(layerField);
        hb3.Add(layerFieldEnableBtn);

        SpaceCtrl space3_1 = new SpaceCtrl();
        space3_1.CurrValue = 20f;
        hb3.Add(space3_1);

        hb3.Add(listCtrl);
        hb3.Add(listCtrlEnableBtn);

        SpaceCtrl space3_2 = new SpaceCtrl();
        space3_2.CurrValue = 20f;
        hb3.Add(space3_2);

        hb3.Add(maskField);
        hb3.Add(maskFieldEnableBtn);

        SpaceCtrl space3_3 = new SpaceCtrl();
        space3_3.CurrValue = 20f;
        hb3.Add(space3_3);

        hb3.Add(mimmaxSlider);
        hb3.Add(mimmaxSliderEnableBtn);

        SpaceCtrl space3_4 = new SpaceCtrl();
        space3_4.CurrValue = 20f;
        hb3.Add(space3_4);

        hb3.Add(objectField);
        hb3.Add(objectFieldEnableBtn);

        SpaceCtrl space3_5 = new SpaceCtrl();
        space3_5.CurrValue = 20f;
        hb3.Add(space3_5);

        hb3.Add(passwordField);
        hb3.Add(passwordFieldEnableBtn);



        #endregion

        #region AddHb4
        hb4.Add(rectField);
        hb4.Add(rectFieldEnableBtn);

        SpaceCtrl space4_1 = new SpaceCtrl();
        space4_1.CurrValue = 20f;
        hb4.Add(space4_1);

        hb4.Add(selectionGrid);
        hb4.Add(selectionGridEnableBtn);

        SpaceCtrl space4_2 = new SpaceCtrl();
        space4_2.CurrValue = 20f;
        hb4.Add(space4_2);

        hb4.Add(tagField);
        hb4.Add(tagFieldEnableBtn);

        SpaceCtrl space4_3 = new SpaceCtrl();
        space4_3.CurrValue = 20f;
        hb4.Add(space4_3);

        hb4.Add(textArea);
        hb4.Add(textAreaEnableBtn);

        SpaceCtrl space4_4 = new SpaceCtrl();
        space4_4.CurrValue = 20f;
        hb4.Add(space4_4);

        hb4.Add(textBox);
        hb4.Add(textBoxEnableBtn);
        #endregion

        #region AddHb5
        hb5.Add(toggleCtrl);
        hb5.Add(toggleCtrlEnableBtn);

        SpaceCtrl space5_1 = new SpaceCtrl();
        space5_1.CurrValue = 20f;
        hb5.Add(space5_1);

        hb5.Add(toolBar);
        hb5.Add(toolBarEnableBtn);

        SpaceCtrl space5_2 = new SpaceCtrl();
        space5_2.CurrValue = 20f;
        hb5.Add(space5_2);

        hb5.Add(vector2Field);
        hb5.Add(vector2FieldEnableBtn);

        SpaceCtrl space5_3 = new SpaceCtrl();
        space5_3.CurrValue = 20f;
        hb5.Add(space5_3);

        hb5.Add(vector3Field);
        hb5.Add(vector3FieldEnableBtn);

        SpaceCtrl space5_4 = new SpaceCtrl();
        space5_4.CurrValue = 20f;
        hb5.Add(space5_4);

        hb5.Add(vector4Field);
        hb5.Add(vector4FieldEnableBtn);

        #endregion

        #region AddHb6
        hb6.Add(playCtrl);
        hb6.Add(playCtrlEnableBtn);
        #endregion

        #region AddVb7
        VBoxCtrl timeLineVB = new VBoxCtrl(true);
        timeLineVB.Size = new Rect(0, 0, 900, 400);

        //vb7.Add(timeLineVB);
        //timeLineVB.Add(timeLineCtrl);
        //timeLineVB.Add(timeLineCtrlEnableBtn);

        vb7.Add(timeLineCtrl);
        vb7.Add(timeLineCtrlEnableBtn);
        #endregion

        #region AddHb8
        hb8.Add(hb5EnableBtn);
        hb8.Add(vb7EnableBtn);
        hb8.Add(changeV2ValueBtn);
        hb8.Add(changeIntValueBtn);
        #endregion

        #region AddHb9
        hb9.Add(labelCtrl);
        hb9.Add(repeatButton);
        #endregion


        m_EditorRoot.RootCtrl = hs1;

        AddListItem();
    }

    static private void OnTestBtnClick(EditorControl c)
    {
        ButtonCtrl testBtn = c as ButtonCtrl;
        if (null == testBtn)
        {
            return;
        }

        EditorUtility.DisplayDialog("测试", "Button可用", "确认");
    }

    static private void OnEnableBtnClick(EditorControl c)
    {
        ButtonCtrl btn = c as ButtonCtrl;
        if (null == btn)
        {
            return;
        }

        EditorControl targetCtrl = btn.UserDefData as EditorControl;

        if (targetCtrl != null)
        {
            targetCtrl.Enable = !targetCtrl.Enable;
            if (targetCtrl.Enable)
            {
                btn.Caption = "无效";
            }
            else
            {
                btn.Caption = "有效";
            }
        }
    }

    static private void OnChangeValueBtnClick(EditorControl c)
    {
        ButtonCtrl btn = c as ButtonCtrl;
        if (null == btn)
        {
            return;
        }

        DataFieldCtrl<Vector2> vector2Field = m_EditorRoot.FindControl("_Vector2Field") as DataFieldCtrl<Vector2>;
        if (vector2Field != null)
        {
            vector2Field.CurrValue = new Vector2(10, 10);
        }

    }

    static private void OnChangeIntValueBtnClick(EditorControl c)
    {
        ButtonCtrl btn = c as ButtonCtrl;
        if (null == btn)
        {
            return;
        }

        DataFieldCtrl<int> intField = m_EditorRoot.FindControl("_IntField") as DataFieldCtrl<int>;
        if (intField != null)
        {
            intField.CurrValue = 111;
        }

    }
    static private void AddListItem()
    {
        ListViewCtrl listCtrl = m_EditorRoot.FindControl("_ListCtrl") as ListViewCtrl;
        if (null == listCtrl)
        {
            return;
        }

        for (int i = 0; i < 10; i++)
        {
            ListCtrlItem newItem = new ListCtrlItem();
            newItem.name = "Test" + i.ToString();

            listCtrl.AddItem(newItem);
        }
    }

    static private void OnListDoubleClick(EditorControl c, object clickObject)
    {

        EditorUtility.DisplayDialog("LabelCtrl", "Item" + (int)clickObject + "双击", "确认");

    }

    static private void OnListOnPressClick(EditorControl c, object clickObject)
    {
        PlayCtrl palyCtrl = m_EditorRoot.FindControl("_PlayCtrl") as PlayCtrl;
        if (null == palyCtrl)
        {
            return;
        }

        palyCtrl.PlayTime += 0.1f;
    }

    static private void OnHboxDoubleClick(EditorControl c, object clickObject)
    {
        EditorUtility.DisplayDialog("HBox", "HBox双击", "确认");
    }

    static private void OnHboxClick(EditorControl c)
    {
        EditorUtility.DisplayDialog("HBox", "HBox单击", "确认");
    }

    static private void OnHboxPress(EditorControl c, object clickObject)
    {
        PlayCtrl palyCtrl = m_EditorRoot.FindControl("_PlayCtrl") as PlayCtrl;
        if(null == palyCtrl)
        {
            return;
        }

        palyCtrl.PlayTime += 0.1f;
    }

    static private void OnVboxDoubleClick(EditorControl c, object clickObject)
    {
        EditorUtility.DisplayDialog("VBox", "VBox双击", "确认");
    }

    static private void OnVboxPress(EditorControl c, object clickObject)
    {
        PlayCtrl palyCtrl = m_EditorRoot.FindControl("_PlayCtrl") as PlayCtrl;
        if (null == palyCtrl)
        {
            return;
        }

        palyCtrl.PlayTime += 0.1f;
    }

    static private void OnVboxClick(EditorControl c)
    {
        EditorUtility.DisplayDialog("VBox", "VBox单击", "确认");
    }

    static private void OnLabelDoubleClick(EditorControl c, object clickObject)
    {
        EditorUtility.DisplayDialog("LabelCtrl", "LabelCtrl双击", "确认");
    }

    static private void OnLabelClick(EditorControl c)
    {
        EditorUtility.DisplayDialog("LabelCtrl", "LabelCtrl单击", "确认");
    }

    static private void OnLabelPress(EditorControl c, object clickObject)
    {
        PlayCtrl palyCtrl = m_EditorRoot.FindControl("_PlayCtrl") as PlayCtrl;
        if (null == palyCtrl)
        {
            return;
        }

        palyCtrl.PlayTime += 0.1f;
    }

    static private void OnRepeatButtonPress(EditorControl c)
    {
        PlayCtrl palyCtrl = m_EditorRoot.FindControl("_PlayCtrl") as PlayCtrl;
        if (null == palyCtrl)
        {
            return;
        }

        palyCtrl.PlayTime += 0.1f;
    }

    static void RequestRepaint()
    {
        m_EditorRoot.RequestRepaint();
    }
}