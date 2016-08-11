using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;

public class CtrlFocusTestEditor
{
    static EditorRoot m_EditorRoot = null;                        //根控件

    [UnityEditor.MenuItem("Assets/H3D/控件测试工具/控件Focus测试")]
    [UnityEditor.MenuItem("H3D/UI/控件测试工具/控件Focus测试")]

    static void Init()
    {
        EditorRoot root = EditorManager.GetInstance().FindEditor("控件Focus测试");
        if (root == null)
        {
            EditorManager.GetInstance().RemoveEditor("控件Focus测试");
            root = EditorManager.GetInstance().CreateEditor("控件Focus测试", false, InitControls);
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

        Rect sliderRect = new Rect(0, 0, 300, 20);
        Rect textRect = new Rect(0, 0, 300, 20);
        Rect filedRect = new Rect(0, 0, 300, 20);
        Rect boundsRect = new Rect(0, 0, 200, 80);
        Rect labelRect = new Rect(0, 0, 200, 80);
        Rect btnRect = new Rect(0, 0, 100, 20);

        HSpliterCtrl hs1 = new HSpliterCtrl();
        hs1.layoutConstraint = LayoutConstraint.GetSpliterConstraint(50f);

        HSpliterCtrl hs2 = new HSpliterCtrl();
        hs2.layoutConstraint = LayoutConstraint.GetSpliterConstraint(50f);

        HSpliterCtrl hs3 = new HSpliterCtrl();
        hs3.layoutConstraint = LayoutConstraint.GetSpliterConstraint(50f);

        HSpliterCtrl hs4 = new HSpliterCtrl();
        hs4.layoutConstraint = LayoutConstraint.GetSpliterConstraint(50f);
  
        HSpliterCtrl hs5 = new HSpliterCtrl();
        hs5.layoutConstraint = LayoutConstraint.GetSpliterConstraint(200f);

        HSpliterCtrl hs6 = new HSpliterCtrl();
        hs6.layoutConstraint = LayoutConstraint.GetSpliterConstraint(200f);

        HBoxCtrl hb1 = new HBoxCtrl(true);
        HBoxCtrl hb2 = new HBoxCtrl(true);
        HBoxCtrl hb3 = new HBoxCtrl(true);
        hb3.Name = "HBox3";
        HBoxCtrl hb4 = new HBoxCtrl(true);
        VBoxCtrl vb5 = new VBoxCtrl(true);
        vb5.Name = "VBox5";
        VBoxCtrl vbSum = new VBoxCtrl(false);

        HBoxCtrl hb6 = new HBoxCtrl(false);
        HBoxCtrl hb7 = new HBoxCtrl(false);

        SliderCtrl<float> floatSlider = new SliderCtrl<float>(0);
        floatSlider.Name = "_FloatSlider";
        floatSlider.Caption = "float值:";
        floatSlider.Size = sliderRect;
        floatSlider.ValueRange = new Vector2(0, 20);

        ButtonCtrl floatSliderValueBtn = new ButtonCtrl();
        floatSliderValueBtn.Caption = "ChangeValue";
        floatSliderValueBtn.Name = "_ChangeFloatSliderValue";
        floatSliderValueBtn.onClick = OnChangeFloatSliderClick;

        SliderCtrl<int> intSlider = new SliderCtrl<int>(0);
        intSlider.Name = "_IntSlider";
        intSlider.Caption = "int值:";
        intSlider.Size = sliderRect;
        intSlider.ValueRange = new Vector2(0, 20);

        ButtonCtrl intSliderValueBtn = new ButtonCtrl();
        intSliderValueBtn.Caption = "ChangeValue";
        intSliderValueBtn.Name = "_ChangeIntSliderValue";
        intSliderValueBtn.onClick = OnChangeIntSliderClick;
        intSliderValueBtn.Enable = true;

        TextBoxCtrl textBox = new TextBoxCtrl();
        textBox.Name = "_TextBox";
        textBox.Text = "AAA";
        textBox.Caption = "TextEdit";
        textBox.Size = textRect;

        ButtonCtrl textBoxValueBtn = new ButtonCtrl();
        textBoxValueBtn.Caption = "ChangeValue";
        textBoxValueBtn.Name = "_ChangeTextBoxValue";
        textBoxValueBtn.onClick = OnChangeTextBoxClick;

        PasswordFieldCtrl passCtrl = new PasswordFieldCtrl();
        passCtrl.Name = "_PassWord";
        passCtrl.Caption = "PassWord";
        passCtrl.Size = textRect;

        ButtonCtrl passWordValueBtn = new ButtonCtrl();
        passWordValueBtn.Caption = "ChangeValue";
        passWordValueBtn.Name = "_ChangePassWordValue";
        passWordValueBtn.onClick = OnChangePassWordClick;


        DataFieldCtrl<int> intField = new DataFieldCtrl<int>(0);
        intField.Name = "_IntField";
        intField.Caption = "Int值:";
        intField.Size = filedRect;
        intField.CurrValue = 1;

        ButtonCtrl intFieldValueBtn = new ButtonCtrl();
        intFieldValueBtn.Caption = "ChangeValue";
        intFieldValueBtn.Name = "_ChangeIntFieldValue";
        intFieldValueBtn.onClick = OnChangeIntFieldClick;

        DataFieldCtrl<float> floatField = new DataFieldCtrl<float>(0);
        floatField.Name = "_FloatField";
        floatField.Caption = "float值:";
        floatField.Size = filedRect;

        ButtonCtrl floatFieldValueBtn = new ButtonCtrl();
        floatFieldValueBtn.Caption = "ChangeValue";
        floatFieldValueBtn.Name = "_ChangeFloatFieldValue";
        floatFieldValueBtn.onClick = OnChangeFloatFieldClick;

        DataFieldCtrl<Rect> rectField = new DataFieldCtrl<Rect>(new Rect(0, 0, 0, 0));
        rectField.Name = "_RectField";
        rectField.Caption = "Rect值:";
        rectField.Size = filedRect;

        ButtonCtrl rectFieldValueBtn = new ButtonCtrl();
        rectFieldValueBtn.Caption = "ChangeValue";
        rectFieldValueBtn.Name = "_ChangeRectFieldValue";
        rectFieldValueBtn.onClick = OnChangeRectFieldClick;

        DataFieldCtrl<Vector2> vector2Field = new DataFieldCtrl<Vector2>(new Vector2(0, 0));
        vector2Field.Name = "_Vector2Field";
        vector2Field.Caption = "Vecetor2值:";
        vector2Field.Size = filedRect;

        ButtonCtrl vector2FieldValueBtn = new ButtonCtrl();
        vector2FieldValueBtn.Caption = "ChangeValue";
        vector2FieldValueBtn.Name = "_ChangeVector2FieldValue";
        vector2FieldValueBtn.onClick = OnChangeVector2FieldClick;

        DataFieldCtrl<Vector3> vector3Field = new DataFieldCtrl<Vector3>();
        vector3Field.Name = "_Vector3Field";
        vector3Field.Caption = "Vecetor3值:";
        vector3Field.Size = filedRect;

        ButtonCtrl vector3FieldValueBtn = new ButtonCtrl();
        vector3FieldValueBtn.Caption = "ChangeValue";
        vector3FieldValueBtn.Name = "_ChangeVector3FieldValue";
        vector3FieldValueBtn.onClick = OnChangeVector3FieldClick;

        DataFieldCtrl<Vector4> vector4Field = new DataFieldCtrl<Vector4>(new Vector4(0, 0, 0, 0));
        vector4Field.Name = "_Vector4Field";
        vector4Field.Caption = "Vecetor4值:";
        vector4Field.Size = filedRect;

        ButtonCtrl vector4FieldValueBtn = new ButtonCtrl();
        vector4FieldValueBtn.Caption = "ChangeValue";
        vector4FieldValueBtn.Name = "_ChangeVector4FieldValue";
        vector4FieldValueBtn.onClick = OnChangeVector4FieldClick;

        DataFieldCtrl<Bounds> boundField = new DataFieldCtrl<Bounds>();
        boundField.Name = "_BoundField";
        boundField.Caption = "Bound值:";
        boundField.Size = boundsRect;

        ButtonCtrl boundFieldValueBtn = new ButtonCtrl();
        boundFieldValueBtn.Caption = "ChangeValue";
        boundFieldValueBtn.Name = "_ChangeBoundFieldValue";
        boundFieldValueBtn.onClick = OnChangeBoundFieldClick;

        LabelCtrl staticLabel = new LabelCtrl();
        staticLabel.Name = "_StaticLabel";
        staticLabel.Caption = "StaticLabel";
        staticLabel.Size = labelRect;

        ButtonCtrl add10SpaceBtn = new ButtonCtrl();
        add10SpaceBtn.Name = "_Add10SpaceBtn";
        add10SpaceBtn.Caption = "AddSpace(10f)";
        add10SpaceBtn.Size = btnRect;
        add10SpaceBtn.onClick = OnAddSpace10Click;

        ButtonCtrl add20SpaceBtn = new ButtonCtrl();
        add20SpaceBtn.Name = "_Add20SpaceBtn";
        add20SpaceBtn.Caption = "AddSpace(20f)";
        add20SpaceBtn.Size = btnRect;
        add20SpaceBtn.onClick = OnAddSpace20Click;

        ButtonCtrl add0SpaceBtn = new ButtonCtrl();
        add0SpaceBtn.Name = "_Add0SpaceBtn";
        add0SpaceBtn.Caption = "AddSpace(0f)";
        add0SpaceBtn.Size = btnRect;
        add0SpaceBtn.onClick = OnAddSpace0Click;

        ButtonCtrl addNegSpaceBtn = new ButtonCtrl();
        addNegSpaceBtn.Name = "_AddNegSpaceBtn";
        addNegSpaceBtn.Caption = "AddSpace(-20f)";
        addNegSpaceBtn.Size = btnRect;
        addNegSpaceBtn.onClick = OnAddSpaceNegativeClick;

        ButtonCtrl remove10SpaceBtn = new ButtonCtrl();
        remove10SpaceBtn.Name = "_Remove10SpaceBtn";
        remove10SpaceBtn.Caption = "RemoveSpace(10f)";
        remove10SpaceBtn.Size = btnRect;
        remove10SpaceBtn.onClick = OnRemoveSpace10Click;

        ButtonCtrl remove20SpaceBtn = new ButtonCtrl();
        remove20SpaceBtn.Name = "_Remove20SpaceBtn";
        remove20SpaceBtn.Caption = "RemoveSpace(20f)";
        remove20SpaceBtn.Size = btnRect;
        remove20SpaceBtn.onClick = OnRemoveSpace20Click;

        DataFieldCtrl<int> intField1 = new DataFieldCtrl<int>();
        intField1.Name = "_IntField1";
        intField1.Caption = "Int值:";
        //intField1.Size = new Rect(0, 0, 300, 20); ;

        DataFieldCtrl<int> intField2 = new DataFieldCtrl<int>();
        intField2.Name = "_IntField2";
        intField2.Caption = "Int值:";
       // intField2.Size = new Rect(0, 0, 300, 80); ;

  
        ButtonCtrl testBtn = new ButtonCtrl();
        testBtn.Name = "_TestBtn";
        testBtn.Caption = "testBtn";
        testBtn.Size = btnRect;

        ButtonCtrl testBtn2 = new ButtonCtrl();
        testBtn2.Name = "_TestBtn2";
        testBtn2.Caption = "testBtn2";
        testBtn2.Size = btnRect;

        ButtonCtrl testBtn3 = new ButtonCtrl();
        testBtn3.Name = "_TestBtn3";
        testBtn3.Caption = "testBtn3";
        testBtn3.Size = btnRect;

        ButtonCtrl testBtn4 = new ButtonCtrl();
        testBtn4.Name = "_TestBtn4";
        testBtn4.Caption = "testBtn4";
        testBtn4.Size = btnRect;

        LabelCtrl label1Ctrl = new LabelCtrl();
        label1Ctrl.Caption = "Test中文";
        label1Ctrl.Name = "_Label1Ctrl";
        //label1Ctrl.Size = btnRect;
        label1Ctrl.fontSize = 12;

        ButtonCtrl add10SpaceBtnV = new ButtonCtrl();
        add10SpaceBtnV.Name = "_Add10SpaceBtnV";
        add10SpaceBtnV.Caption = "AddSpaceV(10f)";
        add10SpaceBtnV.Size = btnRect;
        add10SpaceBtnV.onClick = OnAddSpace10VClick;

        ButtonCtrl add20SpaceBtnV = new ButtonCtrl();
        add20SpaceBtnV.Name = "_Add20SpaceBtnV";
        add20SpaceBtnV.Caption = "AddSpaceV(20f)";
        add20SpaceBtnV.Size = btnRect;
        add20SpaceBtnV.onClick = OnAddSpace20VClick;

        ButtonCtrl add0SpaceBtnV = new ButtonCtrl();
        add0SpaceBtnV.Name = "_Add0SpaceBtnV";
        add0SpaceBtnV.Caption = "AddSpaceV(0f)";
        add0SpaceBtnV.Size = btnRect;
        add0SpaceBtnV.onClick = OnAddSpace0VClick;

        ButtonCtrl addNegSpaceBtnV = new ButtonCtrl();
        addNegSpaceBtnV.Name = "_AddNegSpaceBtn";
        addNegSpaceBtnV.Caption = "AddSpace(-20f)";
        addNegSpaceBtnV.Size = btnRect;
        addNegSpaceBtnV.onClick = OnAddSpaceNegativeVClick;

        ButtonCtrl remove10SpaceBtnV = new ButtonCtrl();
        remove10SpaceBtnV.Name = "_Remove10SpaceBtn";
        remove10SpaceBtnV.Caption = "RemoveSpace(10f)";
        remove10SpaceBtnV.Size = btnRect;
        remove10SpaceBtnV.onClick = OnRemoveSpace10VClick;

        ButtonCtrl remove20SpaceBtnV = new ButtonCtrl();
        remove20SpaceBtnV.Name = "_Remove20SpaceVBtn";
        remove20SpaceBtnV.Caption = "RemoveSpaceV(20f)";
        remove20SpaceBtnV.Size = btnRect;
        remove20SpaceBtnV.onClick = OnRemoveSpace20VClick;

        DataFieldCtrl<int> intField3 = new DataFieldCtrl<int>();
        intField3.Name = "_IntField1";
        intField3.Caption = "Int值:";
        //intField3.Size = new Rect(0, 0, 400, 20); ;

        hs1.Add(hb1);
        hs1.Add(hs2);

        hs2.Add(hb2);
        hs2.Add(hs3);

        hs3.Add(hb3);
        hs3.Add(hs4);

        hs4.Add(hb4);
        hs4.Add(hs5);

        hs5.Add(vb5);
        hs5.Add(hs6);

        hs6.Add(vbSum);

        vbSum.Add(hb6);
        vbSum.Add(hb7);

        hb1.Add(floatSlider);
        hb1.Add(floatSliderValueBtn);

        hb1.Add(intSlider);
        hb1.Add(intSliderValueBtn);

        hb1.Add(intField);
        hb1.Add(intFieldValueBtn);

        hb1.Add(floatField);
        hb1.Add(floatFieldValueBtn);

        hb1.Add(rectField);
        hb1.Add(rectFieldValueBtn);

        hb2.Add(vector2Field);
        hb2.Add(vector2FieldValueBtn);

        hb2.Add(vector3Field);
        hb2.Add(vector3FieldValueBtn);

        hb2.Add(vector4Field);
        hb2.Add(vector4FieldValueBtn);

        hb2.Add(boundField);
        hb2.Add(boundFieldValueBtn);

        hb3.Add(passCtrl);
        hb3.Add(passWordValueBtn);

        hb3.Add(textBox);
        hb3.Add(textBoxValueBtn);

        hb4.Add(staticLabel);
        hb4.Add(add10SpaceBtn);
        hb4.Add(add20SpaceBtn);
        hb4.Add(add0SpaceBtn);
        hb4.Add(addNegSpaceBtn);
        hb4.Add(remove10SpaceBtn);
        hb4.Add(remove20SpaceBtn);

        //HBoxCtrl testBtnHB = new HBoxCtrl();
        //HBoxCtrl testBtnHB2 = new HBoxCtrl();

        //VBoxCtrl intField1VB = new VBoxCtrl();
        //VBoxCtrl intField2VB = new VBoxCtrl();

       // vb5.Add(testBtnHB);
        vb5.Add(intField1);
        vb5.Add(intField2);

        //testBtnHB.Add(testBtn);
        //testBtnHB.Add(testBtn2);

        //testBtnHB2.Add(testBtn3);
        //testBtnHB2.Add(testBtn4);

        //intField1VB.Add(testBtnHB);
        //intField1VB.Add(intField1);


        //intField2VB.Add(testBtnHB2);
        //intField2VB.Add(intField2);

        hb6.Add(label1Ctrl);
        hb6.Add(add10SpaceBtnV);
        hb6.Add(add20SpaceBtnV);
        hb6.Add(add0SpaceBtnV);
        hb6.Add(addNegSpaceBtnV);
        hb6.Add(remove10SpaceBtnV);
        hb6.Add(remove20SpaceBtnV);

        //hb7.Add(intField3);

        m_EditorRoot.RootCtrl = hs1;

        m_EditorRoot.onGUI = OnEditorGUI;
    }

    static void OnEditorGUI(EditorRoot root)
    {
        //if (
        //        (Event.current.clickCount == 1)
        //    && (Event.current.type == EventType.mouseDown)
        //    )
        //{
        //    Debug.Log(Event.current.clickCount);
        //}
        //if (
        //        (Event.current.clickCount == 2)
        //    && (Event.current.type == EventType.mouseDown)
        //    )
        //{
        //    Debug.Log(Event.current.clickCount);
        //}
    }
    static void InitCtrlSize()
    {

    }

    static private void OnChangeIntSliderClick(EditorControl c)
    {
        ButtonCtrl testBtn = c as ButtonCtrl;
        if (null == testBtn)
        {
            return;
        }

        SliderCtrl<int> intSlider = m_EditorRoot.FindControl("_IntSlider") as SliderCtrl<int>;
        if (intSlider != null)
        {
            intSlider.CurrValue = 5;
        }
    }

    static private void OnChangeFloatSliderClick(EditorControl c)
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

        SliderCtrl<float> floatSlider = m_EditorRoot.FindControl("_FloatSlider") as SliderCtrl<float>;
        if (floatSlider != null)
        {
            floatSlider.CurrValue = 5.0f;
        }
    }

    static private void OnChangeTextBoxClick(EditorControl c)
    {
        ButtonCtrl testBtn = c as ButtonCtrl;
        if (null == testBtn)
        {
            return;
        }

        TextBoxCtrl textBox = m_EditorRoot.FindControl("_TextBox") as TextBoxCtrl;
        if (textBox != null)
        {
            textBox.Text = "CCC";
        }

        //DataFieldCtrl<int> intField = m_EditorRoot.FindControl("_IntField") as DataFieldCtrl<int>;
        //if (intField != null)
        //{
        //    intField.CurrValue = 111;
        //}

    }

    static private void OnChangePassWordClick(EditorControl c)
    {
        ButtonCtrl testBtn = c as ButtonCtrl;
        if (null == testBtn)
        {
            return;
        }

        PasswordFieldCtrl passCtrl = m_EditorRoot.FindControl("_PassWord") as PasswordFieldCtrl;
        if (passCtrl != null)
        {
            passCtrl.CurrValue = "CCC";
        }

        //DataFieldCtrl<int> intField = m_EditorRoot.FindControl("_IntField") as DataFieldCtrl<int>;
        //if (intField != null)
        //{
        //    intField.CurrValue = 111;
        //}

    }

    static private void OnChangeIntFieldClick(EditorControl c)
    {
        ButtonCtrl testBtn = c as ButtonCtrl;
        if (null == testBtn)
        {
            return;
        }

        DataFieldCtrl<int> intField = m_EditorRoot.FindControl("_IntField") as DataFieldCtrl<int>;
        if (intField != null)
        {
            intField.CurrValue = 111;
        }
    }

    static private void OnChangeFloatFieldClick(EditorControl c)
    {
        ButtonCtrl testBtn = c as ButtonCtrl;
        if (null == testBtn)
        {
            return;
        }

        DataFieldCtrl<float> floatField = m_EditorRoot.FindControl("_FloatField") as DataFieldCtrl<float>;
        if (floatField != null)
        {
            floatField.CurrValue = 111f;
        }
    }

    static private void OnChangeRectFieldClick(EditorControl c)
    {
        ButtonCtrl testBtn = c as ButtonCtrl;
        if (null == testBtn)
        {
            return;
        }

        DataFieldCtrl<Rect> rectField = m_EditorRoot.FindControl("_RectField") as DataFieldCtrl<Rect>;
        if (rectField != null)
        {
            rectField.CurrValue = new Rect(10, 10, 10, 10);
        }
    }

    static private void OnChangeVector2FieldClick(EditorControl c)
    {
        ButtonCtrl testBtn = c as ButtonCtrl;
        if (null == testBtn)
        {
            return;
        }

        DataFieldCtrl<Vector2> vector2Field = m_EditorRoot.FindControl("_Vector2Field") as DataFieldCtrl<Vector2>;
        if (vector2Field != null)
        {
            vector2Field.CurrValue = new Vector2(10 ,10);
        }
    }

    static private void OnChangeVector3FieldClick(EditorControl c)
    {
        ButtonCtrl testBtn = c as ButtonCtrl;
        if (null == testBtn)
        {
            return;
        }

        DataFieldCtrl<Vector3> vector3Field = m_EditorRoot.FindControl("_Vector3Field") as DataFieldCtrl<Vector3>;
        if (vector3Field != null)
        {
            vector3Field.CurrValue = new Vector3(10, 10, 10);
        }
    }

    static private void OnChangeVector4FieldClick(EditorControl c)
    {
        ButtonCtrl testBtn = c as ButtonCtrl;
        if (null == testBtn)
        {
            return;
        }

        DataFieldCtrl<Vector4> vector4Field = m_EditorRoot.FindControl("_Vector4Field") as DataFieldCtrl<Vector4>;
        if (vector4Field != null)
        {
            vector4Field.CurrValue = new Vector4(10, 10, 10, 10);
        }
    }

    static private void OnChangeBoundFieldClick(EditorControl c)
    {
        ButtonCtrl testBtn = c as ButtonCtrl;
        if (null == testBtn)
        {
            return;
        }

        DataFieldCtrl<Bounds> boundField = m_EditorRoot.FindControl("_BoundField") as DataFieldCtrl<Bounds>;
        if (boundField != null)
        {
            boundField.CurrValue = new Bounds(new Vector2(10, 10), new Vector2(10, 10));
        }
    }

    static private void OnAddSpace10Click(EditorControl c)
    {
        ButtonCtrl testBtn = c as ButtonCtrl;
        if (null == testBtn)
        {
            return;
        }

        SpaceCtrl spaceCtrl = new SpaceCtrl();
        spaceCtrl.CurrValue = 10f;
        spaceCtrl.Name = "Space" + m_SpaceCount.ToString();
       
        TextBoxCtrl textCtrl = m_EditorRoot.FindControl("_TextBox") as TextBoxCtrl;
        if (textCtrl == null)
        {
            return;
        }

        HBoxCtrl hb3 = m_EditorRoot.FindControl("HBox3") as HBoxCtrl;
        if (hb3 != null)
        {
            hb3.Insert(spaceCtrl, textCtrl);
            m_SpaceCount++;
        }
    }

    static private void OnAddSpace20Click(EditorControl c)
    {
        ButtonCtrl testBtn = c as ButtonCtrl;
        if (null == testBtn)
        {
            return;
        }

        SpaceCtrl spaceCtrl = new SpaceCtrl();
        spaceCtrl.CurrValue = 20f;
        spaceCtrl.Name = "Space" + m_SpaceCount.ToString();

        TextBoxCtrl textCtrl = m_EditorRoot.FindControl("_TextBox") as TextBoxCtrl;
        if (textCtrl == null)
        {
            return;
        }

        HBoxCtrl hb3 = m_EditorRoot.FindControl("HBox3") as HBoxCtrl;
        if (hb3 != null)
        {
            hb3.Insert(spaceCtrl, textCtrl);
            m_SpaceCount++;
        }
    }

    static private void OnAddSpace0Click(EditorControl c)
    {
        ButtonCtrl testBtn = c as ButtonCtrl;
        if (null == testBtn)
        {
            return;
        }

        SpaceCtrl spaceCtrl = new SpaceCtrl();
        spaceCtrl.CurrValue = 0f;
        spaceCtrl.Name = "Space" + m_SpaceCount.ToString();

        TextBoxCtrl textCtrl = m_EditorRoot.FindControl("_TextBox") as TextBoxCtrl;
        if (textCtrl == null)
        {
            return;
        }

        HBoxCtrl hb3 = m_EditorRoot.FindControl("HBox3") as HBoxCtrl;
        if (hb3 != null)
        {
            hb3.Insert(spaceCtrl, textCtrl);
            m_SpaceCount++;
        }
    }

    static private void OnAddSpaceNegativeClick(EditorControl c)
    {
        ButtonCtrl testBtn = c as ButtonCtrl;
        if (null == testBtn)
        {
            return;
        }

        SpaceCtrl spaceCtrl = new SpaceCtrl();
        spaceCtrl.CurrValue = -20f;
        spaceCtrl.Name = "Space" + m_SpaceCount.ToString();

        TextBoxCtrl textCtrl = m_EditorRoot.FindControl("_TextBox") as TextBoxCtrl;
        if (textCtrl == null)
        {
            return;
        }

        HBoxCtrl hb3 = m_EditorRoot.FindControl("HBox3") as HBoxCtrl;
        if (hb3 != null)
        {
            hb3.Insert(spaceCtrl, textCtrl);
            m_SpaceCount++;
        }
    }

    static private void OnAddSpace10VClick(EditorControl c)
    {
        AddSpaceV(c, 10f);
    }

    static private void OnAddSpace20VClick(EditorControl c)
    {
        AddSpaceV(c, 20f);
    }

    static private void OnAddSpace0VClick(EditorControl c)
    {
        AddSpaceV(c, 0f);
    }

    static private void OnAddSpaceNegativeVClick(EditorControl c)
    {
        AddSpaceV(c, -20f);
    }

    static private void OnRemoveSpace10Click(EditorControl c)
    {
        HBoxCtrl hb3 = m_EditorRoot.FindControl("HBox3") as HBoxCtrl;

        RemoveSpaceV(hb3, 10f);
    }

    static private void OnRemoveSpace20Click(EditorControl c)
    {
        HBoxCtrl hb3 = m_EditorRoot.FindControl("HBox3") as HBoxCtrl;

        RemoveSpaceV(hb3, 20f);
    }

    static private void OnRemoveSpace10VClick(EditorControl c)
    {
        VBoxCtrl vb5 = m_EditorRoot.FindControl("VBox5") as VBoxCtrl;

        RemoveSpaceV(vb5, 10f);
    }

    static private void OnRemoveSpace20VClick(EditorControl c)
    {
        VBoxCtrl vb5 = m_EditorRoot.FindControl("VBox5") as VBoxCtrl;

        RemoveSpaceV(vb5, 20f);
    }

    static void AddSpaceV(EditorControl c, float space)
    {
        ButtonCtrl testBtn = c as ButtonCtrl;
        if (null == testBtn)
        {
            return;
        }

        SpaceCtrl spaceCtrl = new SpaceCtrl();
        spaceCtrl.CurrValue = space;
        spaceCtrl.Name = "Space" + m_SpaceCount.ToString();

        DataFieldCtrl<int> intFieldCtrl = m_EditorRoot.FindControl("_IntField2") as DataFieldCtrl<int>;
        if (intFieldCtrl == null)
        {
            return;
        }

        VBoxCtrl vb5 = m_EditorRoot.FindControl("VBox5") as VBoxCtrl;
        if (vb5 != null)
        {
            vb5.Insert(spaceCtrl, intFieldCtrl);
            m_SpaceCountV++;
        }
    }

    static void RemoveSpaceV(EditorControl c, float space)
    {
        if (c != null)
        {
            for (int i = c.GetChildCount(); i > 0; i--)
            {
                EditorControl child = c.GetAt(i);
                if ((child is SpaceCtrl) && (space == (float)child.CurrValue))
                {
                    c.Remove(child);
                    m_SpaceCountV--;
                    break;
                }
            }
        }
    }
    private static int m_SpaceCount = 0;
    private static int m_SpaceCountV = 0;

}