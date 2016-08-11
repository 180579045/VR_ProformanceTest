using UnityEditor;
using UnityEngine;
using System;
using System.Linq;
public class ControlTestEditor
{
    static EditorRoot m_EditorRoot = null;                        //根控件
    private static int m_repeatCount = 0;
    static string m_textBox1Name = string.Empty;

    [UnityEditor.MenuItem("Assets/H3D/控件测试工具/共通控件测试")]
    [UnityEditor.MenuItem("H3D/UI/控件测试工具/共通控件测试")]
    static void Init()
    {
        EditorRoot root = EditorManager.GetInstance().FindEditor("控件测试工具");
        if (root == null)
        {
            EditorManager.GetInstance().RemoveEditor("控件测试工具");
            root = EditorManager.GetInstance().CreateEditor("控件测试工具", false, InitControls);
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
        //m_EditorRoot.position = new Rect(100f, 100f, 1920f, 800f);

        {
            //m_EditorRoot.onEnable = OnEnable;
            //m_EditorRoot.onDisable = OnDisable;
        }

        HSpliterCtrl hs1 = new HSpliterCtrl();
        hs1.layoutConstraint = LayoutConstraint.GetSpliterConstraint(50f);

        HSpliterCtrl hs2 = new HSpliterCtrl();
        hs2.layoutConstraint = LayoutConstraint.GetSpliterConstraint(100f);

        HSpliterCtrl hs3 = new HSpliterCtrl();
        hs3.layoutConstraint = LayoutConstraint.GetSpliterConstraint(150f);

        HSpliterCtrl hs4 = new HSpliterCtrl();
        hs4.layoutConstraint = LayoutConstraint.GetSpliterConstraint(200f);

        HSpliterCtrl hs5 = new HSpliterCtrl();
        hs5.layoutConstraint = LayoutConstraint.GetSpliterConstraint(250f);

        HBoxCtrl hb1 = new HBoxCtrl(true);      //上方菜单栏
        HBoxCtrl hb2 = new HBoxCtrl(true);
        HBoxCtrl hb3 = new HBoxCtrl(true);
        HBoxCtrl hb4 = new HBoxCtrl(true);
        HBoxCtrl hb5 = new HBoxCtrl(true);
        VBoxCtrl hb6 = new VBoxCtrl(true);
        hb6.Name = "TestVbox";

        string[] selStrings = new string[] { "Grid 1", "Grid 2", "Grid 3", "Grid 4" };
        string[] toolBarStrings = new string[] { "ToolBar 1", "ToolBar 2", "ToolBar 3", "ToolBar 4" };
        string[] MFStrings = new string[] { "CanJump", "CanShoot", "CanSwim" };

        Rect selectGridRect = new Rect(0, 0, 200, 20);
        Rect toolBarRect = new Rect(0, 0, 300, 20);
        Rect btnRect = new Rect(0, 0, 100, 20);
        Rect toggleRect = new Rect(0, 0, 80, 20);
        Rect enumMFRect = new Rect(0, 0, 200, 20);
        Rect comboBoxRect = new Rect(0, 0, 100, 20);
        //Rect helpRect = new Rect(0, 0, 80, 20);
        Rect intRect = new Rect(0, 0, 300, 20);
        Rect sliderRect = new Rect(0, 0, 200, 20);
        Rect mimmaxSliderRect = new Rect(0, 0, 200, 80);
        Rect layerFieldRect = new Rect(0, 0, 200, 20);
        Rect objectFieldRet = new Rect(0, 0, 400, 20);
        Rect passWordRect = new Rect(0, 0, 300, 20);
        Rect boundsRect = new Rect(0, 0, 200, 80);
        Rect curveRect = new Rect(0, 0, 200, 20);
        Rect tagRect = new Rect(0, 0, 300, 20);
        Rect textAreaRect = new Rect(0, 0, 300, 200);
        Rect listRect = new Rect(0, 0, 400, 400);

        SelectionGridCtrl selectionGrid = new SelectionGridCtrl();
        selectionGrid.Caption = "SelectionGrid";
        selectionGrid.Name = "_SelectionGrid";
        selectionGrid.XCount = 4;
        //selectionGrid.SelectIndex = 0;
        selectionGrid.Size = selectGridRect;
        selectionGrid.onValueChange = OnSelectionGridChange;
        selectionGrid.AddItem(selStrings.ToList<string>());
        //selectionGrid.CurrValue = 1;

        SelectionGridCtrl selectionGrid1 = new SelectionGridCtrl();
        selectionGrid1.Caption = "SelectionGrid";
        selectionGrid1.Name = "_SelectionGrid";
        selectionGrid1.XCount = 4;
        //selectionGrid1.SelectIndex = 0;
        selectionGrid1.Size = selectGridRect;
        selectionGrid1.onValueChange = OnSelectionGridChange;
        selectionGrid1.AddItem(selStrings.ToList<string>());
        //selectionGrid1.CurrValue = 1;

        SelectionGridCtrl selectionGrid2 = new SelectionGridCtrl();
        selectionGrid2.Caption = "SelectionGrid";
        selectionGrid2.Name = "_SelectionGrid";
        selectionGrid2.XCount = 4;
        //selectionGrid2.SelectIndex = 0;
        selectionGrid2.Size = selectGridRect;
        selectionGrid2.onValueChange = OnSelectionGridChange;
        selectionGrid2.AddItem(selStrings.ToList<string>());
        //selectionGrid2.CurrValue = 1;

        SelectionGridCtrl selectionGrid3 = new SelectionGridCtrl();
        selectionGrid3.Caption = "SelectionGrid";
        selectionGrid3.Name = "_SelectionGrid";
        selectionGrid3.XCount = 4;
        //selectionGrid3.SelectIndex = 0;
        selectionGrid3.Size = selectGridRect;
        selectionGrid3.onValueChange = OnSelectionGridChange;
        selectionGrid3.AddItem(selStrings.ToList<string>());
        //selectionGrid3.CurrValue = 1;

        ButtonCtrl repeatButton1 = new ButtonCtrl();
        repeatButton1.IsRepeat = true;
        repeatButton1.Caption = "RepeatButton1";
        repeatButton1.Name = "_RepeatButton1";
        repeatButton1.Size = btnRect;
        repeatButton1.onClick = onRepeatButtonClick;
        repeatButton1.Enable = false;

        ButtonCtrl repeatButton2 = new ButtonCtrl();
        repeatButton2.IsRepeat = true;
        repeatButton2.Caption = "RepeatButton2";
        repeatButton2.Name = "_RepeatButton2";
        repeatButton2.Size = btnRect;
        repeatButton2.onClick = onRepeatButtonClick;

        ButtonCtrl repeatButton3 = new ButtonCtrl();
        repeatButton3.IsRepeat = true;
        repeatButton3.Caption = "RepeatButton3";
        repeatButton3.Name = "_RepeatButton3";
        repeatButton3.Size = btnRect;
        repeatButton3.onClick = onRepeatButtonClick;

        ButtonCtrl repeatButton4 = new ButtonCtrl();
        repeatButton4.IsRepeat = true;
        repeatButton4.Caption = "RepeatButton4";
        repeatButton4.Name = "_RepeatButton4";
        repeatButton4.Size = btnRect;
        repeatButton4.onClick = onRepeatButtonClick;

        ButtonCtrl repeatButton = new ButtonCtrl();
        //repeatButton.IsRepeat = true;
        repeatButton.Caption = "RepeatButton";
        repeatButton.Name = "_RepeatButton";
        repeatButton.Size = btnRect;
        repeatButton.onClick = onRepeatButtonClick;

        ButtonCtrl repeatButton5 = new ButtonCtrl();
        //repeatButton.IsRepeat = true;
        repeatButton5.Caption = "RepeatButton5";
        repeatButton5.Name = "_RepeatButton5";
        repeatButton5.Size = btnRect;
        repeatButton5.onClick = onRepeatButtonClick;

        ButtonCtrl repeatButton6 = new ButtonCtrl();
        //repeatButton.IsRepeat = true;
        repeatButton6.Caption = "RepeatButton6";
        repeatButton6.Name = "_RepeatButton6";
        repeatButton6.Size = btnRect;
        repeatButton6.onClick = onRepeatButtonClick;

        ButtonCtrl repeatButton7 = new ButtonCtrl();
        //repeatButton.IsRepeat = true;
        repeatButton7.Caption = "RepeatButton7";
        repeatButton7.Name = "_RepeatButton7";
        repeatButton7.Size = btnRect;
        repeatButton7.onClick = onRepeatButtonClick;

        ToggleCtrl toggle = new ToggleCtrl();
        toggle.Name = "_Toggle";
        toggle.Caption = "aa";
        toggle.IsToggleLeft = false;
        //toggle.CurrValue = true;
        toggle.Size = toggleRect;
        toggle.onValueChange = onToggleChange;
        //toggle.CurrValue = true;

        ColorCtrl colorCtrl = new ColorCtrl();
        colorCtrl.Name = "_ColorCtrl";
        colorCtrl.Enable = false;

        ToolBarCtrl toolBar = new ToolBarCtrl();
        toolBar.Name = "_ToolBar";
        toolBar.Size = toolBarRect;
        toolBar.onValueChange = OnToolBarChange;
        toolBar.AddItem(toolBarStrings.ToList<string>());
        //toolBar.CurrValue = 1.0;


        StaticEditorFlags item = StaticEditorFlags.BatchingStatic;
        MaskFieldCtrl<Enum> enumMaskField = new MaskFieldCtrl<Enum>(item);
        //enumMaskField.Caption = "异常";
        enumMaskField.Name = "_EnumMaskField";
        enumMaskField.Size = enumMFRect;
        enumMaskField.onValueChange = OnEnumMaskFieldChange;
        //enumMaskField.CurrValue = StaticEditorFlags.LightmapStatic;
        enumMaskField.CurrValue = StaticEditorFlags.LightmapStatic;

        MaskFieldCtrl<int> maskField = new MaskFieldCtrl<int>(0);
        maskField.Name = "_MaskField";
        maskField.Size = enumMFRect;
        maskField.onValueChange = OnMaskFieldChange;
        maskField.AddItem(MFStrings.ToList<string>());
        //maskField.CurrValue = 1.0;

        ComboBoxCtrl<Enum> enumComboBox = new ComboBoxCtrl<Enum>(item);
        enumComboBox.Name = "_EnumComboBox";
        enumComboBox.Size = comboBoxRect;
        enumComboBox.onValueChange = OnEnumComboBoxChange;
        //enumComboBox.CurrValue = StaticEditorFlags.OccluderStatic;


        HelpBoxCtrl helpBox = new HelpBoxCtrl();
        helpBox.Name = "_HelpBox";
        helpBox.Caption = "警告";
        //helpBox.Size = helpRect;
        helpBox.MsgType = MessageType.Warning;

        DataFieldCtrl<int> intField = new DataFieldCtrl<int>(0);
        intField.Name = "_IntField";
        intField.Caption = "Int值:";
        intField.Size = intRect;
        intField.onValueChange = OnDataFieldChange;
        //intField.CurrValue = 1.0;

        DataFieldCtrl<int> intField2 = new DataFieldCtrl<int>(0);
        intField2.Name = "_IntField2";
        intField2.Caption = "Int2值:";
        intField2.Size = intRect;
        intField2.onValueChange = OnDataFieldChange;
        intField2.CurrValue = 1;

        DataFieldCtrl<Vector2> vector2Field = new DataFieldCtrl<Vector2>(new Vector2(0, 0));
        vector2Field.Name = "_Vector2Field";
        vector2Field.Caption = "Vecetor2值:";
        vector2Field.Size = intRect;
        vector2Field.onValueChange = OnDataFieldChange;

        DataFieldCtrl<float> floatField = new DataFieldCtrl<float>(0);
        floatField.Name = "_FloatField";
        floatField.Caption = "float值:";
        floatField.Size = intRect;
        floatField.onValueChange = OnDataFieldChange;
        floatField.CurrValue = 2.0f;

        DataFieldCtrl<Rect> rectField = new DataFieldCtrl<Rect>(new Rect(0, 0, 0, 0));
        rectField.Name = "_RectField";
        rectField.Caption = "Rect值:";
        rectField.Size = intRect;
        rectField.onValueChange = OnDataFieldChange;
        //rectField.CurrValue = 1;

        DataFieldCtrl<Vector3> vector3Field = new DataFieldCtrl<Vector3>();
        vector3Field.Name = "_Vector3Field";
        vector3Field.Caption = "Vecetor3值:";
        vector3Field.Size = intRect;
        vector3Field.onValueChange = OnDataFieldChange;
        //vector3Field.CurrValue = 1;

        DataFieldCtrl<Vector4> vector4Field = new DataFieldCtrl<Vector4>(new Vector4(0, 0, 0, 0));
        vector4Field.Name = "_Vector4Field";
        vector4Field.Caption = "Vecetor4值:";
        vector4Field.Size = intRect;
        vector4Field.onValueChange = OnDataFieldChange;
        //vector4Field.CurrValue = 1;

        SliderCtrl<float> floatSlider = new SliderCtrl<float>(0);
        floatSlider.Name = "_FloatSlider";
        //floatSlider.Caption = "float值:";
        floatSlider.Size = sliderRect;
        floatSlider.ValueRange = new Vector2(0, 20);
        floatSlider.onValueChange = OnSliderChange;
        //floatSlider.CurrValue = 1;

        SliderCtrl<int> intSlider = new SliderCtrl<int>(0);
        intSlider.Name = "_IntSlider";
        //intSlider.Caption = "int值:";
        //intSlider.Size = sliderRect;
        intSlider.ValueRange = new Vector2(0, 20);
        intSlider.onValueChange = OnSliderChange;
        //intSlider.CurrValue = 1.0;

        SliderCtrl<Vector2> mimmaxSlider = new SliderCtrl<Vector2>();
        mimmaxSlider.Name = "_MinMaxSlider";
        //mimmaxSlider.Caption = "MinMax值:";
        mimmaxSlider.Size = mimmaxSliderRect;
        mimmaxSlider.ValueRange = new Vector2(-20, 20);
        mimmaxSlider.onValueChange = OnSliderChange;

        LayerFieldCtrl layerField = new LayerFieldCtrl();
        layerField.Name = "_LayerField";
        //layerField.Caption = "Layer:";
        layerField.Size = layerFieldRect;
        layerField.onValueChange = OnLayerChange;
        //layerField.CurrValue = 2.1;

        ObjectFieldCtrl objectField = new ObjectFieldCtrl();
        objectField.Name = "_ObjectField";
        objectField.Caption = "Object:";
        objectField.ObjectType = typeof(object);
        objectField.Size = objectFieldRet;
        objectField.onValueChange = OnObjectChange;

        PasswordFieldCtrl passwordField = new PasswordFieldCtrl();
        passwordField.Name = "_PasswordField";
        passwordField.Caption = "Password:";
        passwordField.Size = passWordRect;
        passwordField.onValueChange = OnPasswordChange;
        //passwordField.CurrValue = 1;

        //Bounds initValue = new Bounds(Vector3.zero, new Vector3(10, 20, 10));
        DataFieldCtrl<Bounds> boundsField = new DataFieldCtrl<Bounds>();
        boundsField.Name = "_BoundsField";
        boundsField.Caption = "Bounds:";
        boundsField.Size = boundsRect;
        boundsField.onValueChange = OnDataFieldChange;
        //boundsField.CurrValue = 1;
        boundsField.Enable = false;

        CurveEditorCtrl curveEditor = new CurveEditorCtrl(AnimationCurve.Linear(0, 0, 10, 10));
        curveEditor.Name = "_CurveEditor";
        curveEditor.Caption = "AniCurve";
        curveEditor.Size = curveRect;
        curveEditor.onValueChange = OnCurveChange;
        //curveEditor.CurveRange = new Rect(0, 0, 10, 10);
        //curveEditor.CurrValue = 1;

        TagFieldCtrl tagField = new TagFieldCtrl();
        tagField.Name = "_TagField";
        tagField.Caption = "Tag:";
        tagField.Size = tagRect;
        tagField.onValueChange = OnTagChange;

        TextAreaCtrl textArea = new TextAreaCtrl();
        textArea.Name = "_TextArea";
        textArea.Size = textAreaRect;
        textArea.onValueChange = OnTextAreaChange;

        ButtonCtrl testBtn = new ButtonCtrl();
        testBtn.Name = "_ButtonCtrl";
        testBtn.Caption = "ButtonCtrl";
        testBtn.onClick = OnTestButtonClick;

        TextBoxCtrl textBox = new TextBoxCtrl();
        //textBox.Name = "_TextBox";
        textBox.Text = "AAA";
        textBox.Caption = "TextEdit";
        textBox.Size = curveRect;
        textBox.onValueChange = OnTextBoxChange;
        m_textBox1Name = textBox.Name;

        TextBoxCtrl textBox2 = new TextBoxCtrl();
        //textBox2.Name = "_TextBox2";
        textBox2.Text = "DDD";
        textBox2.Caption = "TextEdit2";
        textBox2.Size = curveRect;
        textBox2.onValueChange = OnTextBoxChange;

        DataFieldCtrl<Bounds> boundsField1 = new DataFieldCtrl<Bounds>();
        boundsField1.Name = "_BoundsField1";
        boundsField1.Caption = "Bounds1:";
        boundsField1.Size = new Rect(0, 0, 400, 100);
        boundsField1.onValueChange = OnDataFieldChange;


        DataFieldCtrl<Bounds> boundsField2 = new DataFieldCtrl<Bounds>();
        boundsField2.Name = "_BoundsField2";
        boundsField2.Caption = "Bounds2:";
        boundsField2.Size = boundsRect;
        boundsField.onValueChange = OnDataFieldChange;

        DataFieldCtrl<Bounds> boundsField3 = new DataFieldCtrl<Bounds>();
        boundsField3.Name = "_BoundsField3";
        boundsField3.Caption = "Bounds3:";
        boundsField3.Size = boundsRect;
        boundsField3.onValueChange = OnDataFieldChange;

        DataFieldCtrl<Bounds> boundsField6 = new DataFieldCtrl<Bounds>();
        boundsField6.Name = "_BoundsField6";
        boundsField6.Caption = "Bounds6:";
        boundsField6.Size = boundsRect;
        boundsField6.onValueChange = OnDataFieldChange;

        DataFieldCtrl<Bounds> boundsField4 = new DataFieldCtrl<Bounds>();
        boundsField4.Name = "_BoundsField4";
        boundsField4.Caption = "Bounds4:";
        boundsField4.Size = boundsRect;
        boundsField4.onValueChange = OnDataFieldChange;

        DataFieldCtrl<Bounds> boundsField5 = new DataFieldCtrl<Bounds>();
        boundsField5.Name = "_BoundsFiel5";
        boundsField5.Caption = "Bounds5:";
        boundsField5.Size = boundsRect;
        boundsField5.onValueChange = OnDataFieldChange;

        VBoxCtrl listVbox = new VBoxCtrl(true);
        listVbox.Name = "_ListVbox";
        listVbox.Size = new Rect(0, 0, 800, 600);

        VBoxCtrl listVboxLever2 = new VBoxCtrl(true);
        listVboxLever2.Name = "_ListVboxLever2";


        ListViewCtrl listCtrl = new ListViewCtrl();
        listCtrl.Name = "_ListCtrl";
        listCtrl.Size = listRect;

        HBoxCtrl sumHb = new HBoxCtrl(true);      //上方菜单栏

        m_EditorRoot.RootCtrl = hs1;

        hs1.Add(sumHb);
        hs1.Add(hs2);

        hs2.Add(hb2);
        hs2.Add(hs3);

        hs3.Add(hb3);
        hs3.Add(hs4);

        hs4.Add(hb4);
        hs4.Add(hs5);

        hs5.Add(hb5);

        hs5.Add(hb6);

        sumHb.Add(repeatButton);
        sumHb.Add(repeatButton1);
        sumHb.Add(repeatButton2);
        sumHb.Add(hb1);
        sumHb.Add(repeatButton3);
        sumHb.Add(repeatButton4);


        hb1.Add(selectionGrid);
        hb1.Add(selectionGrid1);
        hb1.Add(selectionGrid2);
        hb1.Add(selectionGrid3);

        hb1.Add(toolBar);
        hb1.Add(enumMaskField);
        hb1.Add(maskField);
        hb1.Add(enumComboBox);

        hb2.Add(intField);
        hb2.Add(intField2);
        hb2.Add(floatField);
        hb2.Add(vector2Field);
        hb2.Add(rectField);

        hb3.Add(toggle);
        hb3.Add(colorCtrl);
        hb3.Add(helpBox);

        hb4.Add(vector3Field);
        hb4.Add(vector4Field);
        hb4.Add(floatSlider);
        hb4.Add(intSlider);
        hb4.Add(mimmaxSlider);
        // hb4.Add(layerField);

        hb5.Add(layerField);

        hb5.Add(objectField);
        hb5.Add(passwordField);
        hb5.Add(boundsField);
        hb5.Add(curveEditor);
        hb5.Add(tagField);
        hb5.Add(textArea);

        hb6.Add(testBtn);
        hb6.Add(textBox);
        hb6.Add(textBox2);
        hb6.Add(listVbox);
        //hb6.Add(boundsField1);
        hb6.Add(boundsField2);
        hb6.Add(boundsField3);
        hb6.Add(boundsField4);
        hb6.Add(boundsField5);
        hb6.Add(boundsField6);

        listVbox.Add(boundsField1);
        listVbox.Add(listCtrl);

        //listVbox.Add(listVboxLever2);
        //listVboxLever2.Add(boundsField1);
        //listVboxLever2.Add(listCtrl);

        m_EditorRoot.onGUI = OnEditorGUI;


        AddListItem();

    }

    static void onRepeatButtonClick(EditorControl c)
    {
        ButtonCtrl repeatButton = c as ButtonCtrl;
        if(null == repeatButton)
        {
            return;
        }

        m_repeatCount++;

        VBoxCtrl vboxCtrl = m_EditorRoot.FindControl("TestVbox") as VBoxCtrl;
        if (null == vboxCtrl)
        {
            return;
        }

        vboxCtrl.Visiable = !vboxCtrl.Visiable;
        Debug.Log("RepeatCoutn:" + m_repeatCount);

    }
    
    static void OnSelectionGridChange(EditorControl c, object value)
    {
        SelectionGridCtrl selectionGrid = c as SelectionGridCtrl;
        if (null == selectionGrid)
        {
            return;
        }

        if(null == value)
        {
            return;
        }

        Debug.Log(value + " " + selectionGrid.DispStr[(int)value]);
    }

    static void OnEnumMaskFieldChange(EditorControl c, object value)
    {
        MaskFieldCtrl<Enum> enumMaskField = c as MaskFieldCtrl<Enum>;
        if (null == enumMaskField)
        {
            return;
        }

        if (null == value)
        {
            return;
        }

        Debug.Log(value);
    }

    static void OnMaskFieldChange(EditorControl c, object value)
    {
        MaskFieldCtrl<int> maskField = c as MaskFieldCtrl<int>;
        if (null == maskField)
        {
            return;
        }

        if (null == value)
        {
            return;
        }

        Debug.Log(value);
    }

    static void OnToolBarChange(EditorControl c, object value)
    {
        ToolBarCtrl toolBar = c as ToolBarCtrl;
        if (null == toolBar)
        {
            return;
        }

        if (null == value)
        {
            return;
        }

        Debug.Log(value + " " + toolBar.DispStr[(int)value]);
    }

    static void onToggleChange(EditorControl c, object value)
    {
        ToggleCtrl toggle = c as ToggleCtrl;
        if(null == toggle)
        {
            return;
        }

        if(null == value)
        {
            return;
        }

        Debug.Log("Value Change" + (bool)value);
    }

    static private void OnEnumComboBoxChange(EditorControl c, object value)
    {
        ComboBoxCtrl<Enum> enumComboBox = c as ComboBoxCtrl<Enum>;
        if(null == enumComboBox)
        {
            return;
        }

        if (null == value)
        {
            return;
        }

        Debug.Log("Value Change:" + value);
    }

    static private void OnDataFieldChange(EditorControl c, object value)
    {
        if (null == value)
        {
            return;
        }

        do
        {
            DataFieldCtrl<int> intField = c as DataFieldCtrl<int>;
            if (intField != null)
            {
                Debug.Log("Int Value Change:" + value);
                break;
            }

            DataFieldCtrl<Vector2> vector2Field = c as DataFieldCtrl<Vector2>;
            if (vector2Field != null)
            {
                Debug.Log("Vector Value Change:" + value);
                break;
            }

            DataFieldCtrl<float> floatField = c as DataFieldCtrl<float>;
            if (floatField != null)
            {
                Debug.Log("Float Value Change:" + value);
                break;
            }

            DataFieldCtrl<Rect> rectField = c as DataFieldCtrl<Rect>;
            if(rectField != null)
            {
                Debug.Log("Rect Value Change:" + value);
                break;
            }

            DataFieldCtrl<Vector3> vector3Field = c as DataFieldCtrl<Vector3>;
            if (vector3Field != null)
            {
                Debug.Log("Vector3 Value Change:" + value);
                break;
            }

            DataFieldCtrl<Vector4> vector4Field = c as DataFieldCtrl<Vector4>;
            if (vector4Field != null)
            {
                Debug.Log("Vector4 Value Change:" + value);
                break;
            }

            DataFieldCtrl<Bounds> boundsField = c as DataFieldCtrl<Bounds>;
            if (boundsField != null)
            {
                Debug.Log("Bounds Value Change:" + value);
                break;
            }
        } while (false);
    }

    static private void OnSliderChange(EditorControl c, object value)
    {
        if (null == value)
        {
            return;
        }

        do
        {
            SliderCtrl<float> floatSlider = c as SliderCtrl<float>;
            if(floatSlider != null)
            {
                Debug.Log("float Value slide:" + value);
                break;
            }

            SliderCtrl<int> intSilder = c as SliderCtrl<int>;
            if(intSilder != null)
            {
                Debug.Log("int Value slide:" + value);
                break;
            }

            SliderCtrl<Vector2> minmaxSlider = c as SliderCtrl<Vector2>;
            if (minmaxSlider != null)
            {
                Debug.Log("MinMax Value slide:" + value);
                break;
            }
        } while (false);
    }

    static private void OnCurveChange(EditorControl c, object value)
    {
        if (null == value)
        {
            return;
        }

        CurveEditorCtrl curveEditor = c as CurveEditorCtrl;
        if (null == curveEditor)
        {
            return;
        }

        Debug.Log("Curve:" + value);
    }

    static private void OnLayerChange(EditorControl c, object value)
    {
        if (null == value)
        {
            return;
        }

        LayerFieldCtrl layerField = c as LayerFieldCtrl;
        if (null == layerField)
        {
            return;
        }

        Debug.Log("Layer:" + value);
    }

    static private void OnObjectChange(EditorControl c, object value)
    {
        if (null == value)
        {
            return;
        }

        ObjectFieldCtrl objectField = c as ObjectFieldCtrl;
        if (null == objectField)
        {
            return;
        }

        Debug.Log("Object:" + value);
    }

    static private void OnPasswordChange(EditorControl c, object value)
    {
        if (null == value)
        {
            return;
        }

        PasswordFieldCtrl passwordField = c as PasswordFieldCtrl;
        if (null == passwordField)
        {
            return;
        }

        Debug.Log("PassWord:" + value);
    }

    static private void OnTagChange(EditorControl c, object value)
    {
        if (null == value)
        {
            return;
        }

        TagFieldCtrl tagEditor = c as TagFieldCtrl;
        if (null == tagEditor)
        {
            return;
        }

        Debug.Log("Tag:" + value);
    }

    static private void OnTextAreaChange(EditorControl c, object value)
    {
        if (null == value)
        {
            return;
        }

        TextAreaCtrl textArea = c as TextAreaCtrl;
        if (null == textArea)
        {
            return;
        }

        Debug.Log("Text:" + value);
    }

    static private void OnTextBoxChange(EditorControl c, object value)
    {
        TextBoxCtrl textBox = c as TextBoxCtrl;
        if (null == textBox)
        {
            return;
        }

        Debug.Log("Text:" + textBox.Text);
    }

    static private void RequestRepaint()
    {
        if (m_EditorRoot != null)
        {
            m_EditorRoot.RequestRepaint();
        }
    }

    static private void OnTestButtonClick(EditorControl c)
    {
        ButtonCtrl testBtn = c as ButtonCtrl;
        if (null == testBtn)
        {
            return;
        }

        //TextBoxCtrl textBox = m_EditorRoot.FindControl(m_textBox1Name) as TextBoxCtrl;
        //if (textBox != null)
        //{
        //    textBox.Text = "ccc";
        //}

        //DataFieldCtrl<int> intField = m_EditorRoot.FindControl("_IntField") as DataFieldCtrl<int>;
        //if (intField != null)
        //{
        //    intField.CurrValue = 111;
        //}

        SliderCtrl<int> intSlider = m_EditorRoot.FindControl("_IntSlider") as SliderCtrl<int>;
        if (intSlider != null)
        {
            intSlider.CurrValue = 5;
        }
    }
    static void OnEditorGUI(EditorRoot root)
    {//编辑器响应窗口OnGuI回调函数

        if ((Event.current.type == EventType.MouseDrag)
            || (Event.current.type == EventType.ScrollWheel))
        {
            RequestRepaint();

            TextBoxCtrl textBox = root.FindControl(m_textBox1Name) as TextBoxCtrl;
            if(textBox != null)
            {
                textBox.Text = "bbb";
            }
        }
    }

    static private void AddListItem()
    {
        ListViewCtrl listCtrl = m_EditorRoot.FindControl("_ListCtrl") as ListViewCtrl;
        if (null == listCtrl)
        {
            return;
        }

        for (int i = 0; i < 30; i++ )
        {
            ListCtrlItem newItem = new ListCtrlItem();
            newItem.name = "Test" + i.ToString();

            listCtrl.AddItem(newItem);
        }
    }
}