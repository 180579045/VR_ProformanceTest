using UnityEngine;
using System.Collections;
using UnityEditor;
using System;

public class EditorRenderVisitor : EditorCtrlVisitor 
{


    private EditorRenderStrategy labelStrategy = new LabelCtrlRenderStrategy();
    private EditorRenderStrategy btnStrategy = new ButtonRenderStrategy();
    private EditorRenderStrategy hboxStrategy = new HBoxRenderStrategy();
    private EditorRenderStrategy vboxStrategy = new VBoxRenderStrategy();
    private EditorRenderStrategy hspliterStrategy = new HSpliterRenderStrategy();
    private EditorRenderStrategy vspliterStrategy = new VSpliterRenderStrategy();
    private EditorRenderStrategy mainviewStrategy = new MainViewRenderStrategy();
    private EditorRenderStrategy listviewStrategy = new ListViewRenderStrategy();
    private EditorRenderStrategy timelineviewStrategy = new TimeLineViewRenderStrategy();
    private EditorRenderStrategy inspectorStrategy = new InspectorRenderStrategy();
  
    //Modify by liteng for 追加共通控件 at 2015/2/26
    private EditorRenderStrategy floatSliderStrategy = new FloatSliderRenderStrategy();
    private EditorRenderStrategy playctrlStrategy = new PlayCtrlRenderStrategy();
    private EditorRenderStrategy colorctrlStrategy = new ColorCtrlRenderStrategy();
    private EditorRenderStrategy tabviewStrategy = new TabViewRenderStrategy();
    private EditorRenderStrategy treeviewStrategy = new TreeViewRenderStrategy();
    private EditorRenderStrategy comboxStrategy = new ComboBoxRenderStrategy();
    private EditorRenderStrategy textboxStrategy = new TextBoxRenderStrategy();
    //Add by liteng for MoveAtlas At 2014/1/4
    private EditorRenderStrategy textureboxStrategy = new TextureBoxRenderStrategy();

    //Add by liteng for 追加共通控件 at 2015/2/26 Start
    private EditorRenderStrategy selectionGridStrategy = new SelectionGridRenderStrategy();
    private EditorRenderStrategy toggleStrategy = new ToggleRenderStrategy();
    private EditorRenderStrategy toolBarStrategy = new ToolBarRenderStrategy();
    private EditorRenderStrategy enumMaskFieldStrategy = new EnumMaskFieldRenderStrategy();
    private EditorRenderStrategy maskFieldStrategy = new MaskFieldRenderStrategy();
    private EditorRenderStrategy enumComboBoxStrategy = new EnumComboBoxRenderStrategy();
    private EditorRenderStrategy helpBoxStrategy = new HelpBoxRenderStrategy();
    private EditorRenderStrategy intFieldStrategy = new IntFieldRenderStrategy();
    private EditorRenderStrategy vector2FieldStrategy = new Vector2FieldRenderStrategy();
    private EditorRenderStrategy floatFieldStrategy = new FloatFieldRenderStrategy();
    private EditorRenderStrategy rectFieldStrategy = new RectFieldRenderStrategy();
    private EditorRenderStrategy vector3FieldStrategy = new Vector3FieldRenderStrategy();
    private EditorRenderStrategy vector4FieldStrategy = new Vector4FieldRenderStrategy();
    private EditorRenderStrategy intSliderStrategy = new IntSliderRenderStrategy();
    private EditorRenderStrategy minmaxSliderStrategy = new MinMaxSliderRenderStrategy();
    private EditorRenderStrategy layerFieldStrategy = new LayerFieldRenderStrategy();
    private EditorRenderStrategy objectFieldStrategy = new ObjectFieldRenderStrategy();
    private EditorRenderStrategy passwordFieldStrategy = new PasswordFieldRenderStrategy();
    private EditorRenderStrategy boundsFieldStrategy = new BoundsFieldRenderStrategy();
    private EditorRenderStrategy curveEditorStrategy = new CurveEditorRenderStrategy();
    private EditorRenderStrategy tagFieldStrategy = new TagFieldRenderStrategy();
    private EditorRenderStrategy textAreaStrategy = new TextAreaRenderStrategy();
    private EditorRenderStrategy spaceStrategy = new SpaceRenderStrategy();
    //Add by liteng for 追加共通控件 End

    private EditorRenderStrategy _GetStrategy(EditorControl c)
    {
        //Modify by liteng for 代码改善 at 2015/2/26 Start
        if(c is ButtonCtrl)
        {
            return btnStrategy;
        }
        else if (c is HBoxCtrl)
        {
            return hboxStrategy;
        }
        else if (c is VBoxCtrl)
        {
            return vboxStrategy;
        }
        else if (c is HSpliterCtrl)
        {
            return hspliterStrategy;
        }
        else if (c is VSpliterCtrl)
        {
            return vspliterStrategy;
        }
        else if (c is MainViewCtrl)
        {
            return mainviewStrategy;
        }
        else if (c is ListViewCtrl)
        {
            return listviewStrategy;
        }
        else if (c is TimeLineViewCtrl)
        {
            return timelineviewStrategy;
        }
        else if (c is InspectorViewCtrl)
        {
            return inspectorStrategy;
        }
        else if(c is SliderCtrl<float>)
        {
            return floatSliderStrategy;
        }
        else if( c is PlayCtrl)
        {
            return playctrlStrategy;
        }
        else if(c is LabelCtrl)
        {
            return labelStrategy;
        }
        else if (c is ColorCtrl)
        {
            return colorctrlStrategy;
        }
        else if(c is TabViewCtrl)
        {
            return tabviewStrategy;
        }
        else if(c is TreeViewCtrl)
        {
            return treeviewStrategy;
        }
        else if(c is ComboBoxCtrl<int>)
        {
            return comboxStrategy;
        }
        else if(c is TextBoxCtrl)
        {
            return textboxStrategy;
        }
        //Add by liteng for MoveAtlas At 2014/1/4 Start
        else if (c is TextureBoxCtrl)
        {
            return textureboxStrategy;
        }
        //Add by liteng for MoveAtlas End
        //Add by lteng for 追加共通控件 At 2015/2/26 Start
        else if (c is SelectionGridCtrl)
        {
            return selectionGridStrategy;
        }
        else if (c is ToggleCtrl)
        {
            return toggleStrategy;
        }
        else if(c is ToolBarCtrl)
        {
            return toolBarStrategy;
        }
        else if(c is MaskFieldCtrl<Enum>)
        {
            return enumMaskFieldStrategy;
        }
        else if(c is MaskFieldCtrl<int>)
        {
            return maskFieldStrategy;
        }
        else if(c is ComboBoxCtrl<Enum>)
        {
            return enumComboBoxStrategy;
        }
        else if(c is HelpBoxCtrl)
        {
            return helpBoxStrategy;
        }
        else if(c is DataFieldCtrl<int>)
        {
            return intFieldStrategy;
        }
        else if(c is DataFieldCtrl<Vector2>)
        {
            return vector2FieldStrategy;
        }
        else if(c is DataFieldCtrl<float>)
        {
            return floatFieldStrategy;
        }
        else if(c is DataFieldCtrl<Rect>)
        {
            return rectFieldStrategy;
        }
        else if(c is DataFieldCtrl<Vector3>)
        {
            return vector3FieldStrategy;
        }
        else if(c is DataFieldCtrl<Vector4>)
        {
            return vector4FieldStrategy;
        }
        else if(c is SliderCtrl<int>)
        {
            return intSliderStrategy;
        }
        else if(c is SliderCtrl<Vector2>)
        {
            return minmaxSliderStrategy;
        }
        else if(c is LayerFieldCtrl)
        {
            return layerFieldStrategy;
        }
        else if(c is ObjectFieldCtrl)
        {
            return objectFieldStrategy;
        }
        else if(c is PasswordFieldCtrl)
        {
            return passwordFieldStrategy;
        }
        else if(c is DataFieldCtrl<Bounds>)
        {
            return boundsFieldStrategy;
        }
        else if(c is CurveEditorCtrl)
        {
            return curveEditorStrategy;
        }
        else if(c is TagFieldCtrl)
        {
            return tagFieldStrategy;
        }
        else if(c is TextAreaCtrl)
        {
            return textAreaStrategy;
        }
        else if(c is SpaceCtrl)
        {
            return spaceStrategy;
        }
        //Add by liteng for 追加共通控件 End

        //Modify by liteng for 代码改善 End

        return null;
    }
 

    public override bool PreVisit(EditorControl c) 
    {
        EditorRenderStrategy s = _GetStrategy(c);
        if( s != null )
        {
            return s.PreVisit(c);
        }
        return true;
    }
    public override void Visit(EditorControl c) 
    {
        EditorRenderStrategy s = _GetStrategy(c);
        if (s != null)
        {
            s.Visit(c);
        }
    }
    public override void AfterVisit(EditorControl c) 
    {
        EditorRenderStrategy s = _GetStrategy(c);
        if (s != null)
        {
            s.AfterVisit(c);
        }
    }
    public override bool PreVisitChildren(EditorControl c) 
    {
        EditorRenderStrategy s = _GetStrategy(c);
        if (s != null)
        {
            return s.PreVisitChildren(c);
        }
        return true;
    }
    public override void AfterVisitChildren(EditorControl c) 
    {
        EditorRenderStrategy s = _GetStrategy(c);
        if (s != null)
        {
            s.AfterVisitChildren(c);
        }
    }

    public override bool PreVisitChild(EditorControl c, int ichild) 
    {
        EditorRenderStrategy s = _GetStrategy(c);
        if (s != null)
        {
            return s.PreVisitChild(c, ichild);
        }
        return true;
    }
    public override void AfterVisitChild(EditorControl c, int ichild)
    {
        EditorRenderStrategy s = _GetStrategy(c);
        if (s != null)
        {
            s.AfterVisitChild(c, ichild);
        }
    }
 
}
