using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.DisplayUI;

namespace AmherstEngine
{
    public partial class frmMain : Form
    {
        private ESRI.ArcGIS.Controls.IMapControl3 m_mapControl = null;
        private ESRI.ArcGIS.Controls.IPageLayoutControl2 m_pageLayoutControl = null;
        private IMapDocument m_mapDocument;
        private ControlsSynchronizer m_controlsSynchronizer = null;
        private string sMapUnits;

        public frmMain()
        {
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            m_mapControl = (IMapControl3)this.axMapControl1.Object;
            m_pageLayoutControl = (IPageLayoutControl2)this.axPageLayoutControl1.Object;
            m_controlsSynchronizer = new ControlsSynchronizer(m_mapControl, m_pageLayoutControl);
            m_controlsSynchronizer.BindControls(true);
            m_controlsSynchronizer.AddFrameworkControl(axToolbarControl1.Object);
            m_controlsSynchronizer.AddFrameworkControl(axTOCControl1.Object);
            Commands.OpenNewMapDocument openMapDoc = new Commands.OpenNewMapDocument(m_controlsSynchronizer);
            axToolbarControl1.AddItem(openMapDoc, -1, 0, false, -1, esriCommandStyles.esriCommandStyleIconOnly);
            sMapUnits = "Unknow";

        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.tabControl1.SelectedIndex == 0)
                m_controlsSynchronizer.ActivateMap();
            else
                m_controlsSynchronizer.ActivatePageLayout();
        }

        private void axMapControl1_OnMouseMove(object sender, IMapControlEvents2_OnMouseMoveEvent e)
        {
            ScaleLabel.Text = "Scale 1:" + ((long)this.axMapControl1.MapScale).ToString();
            CoordinateLabel.Text = ((long)e.mapX).ToString() + "," + ((long)e.mapY).ToString() + " " + sMapUnits;
        }

        private void axToolbarControl1_OnMouseMove(object sender, IToolbarControlEvents_OnMouseMoveEvent e)
        {
            int index = axToolbarControl1.HitTest(e.x, e.y, false);
            if (index != -1)
            {
                IToolbarItem toolbarItem = axToolbarControl1.GetItem(index);
                if (toolbarItem.Command != null)
                    MessageLabel.Text = toolbarItem.Command.Message;
            }
            else
            {
                MessageLabel.Text = "Ready";
            }
        }

        private void axMapControl1_OnMapReplaced(object sender, IMapControlEvents2_OnMapReplacedEvent e)
        {
            esriUnits mapUnits = axMapControl1.MapUnits;
            switch (mapUnits)
            {
                case esriUnits.esriCentimeters:
                    sMapUnits = "Centimeters";
                    break;
                case esriUnits.esriDecimalDegrees:
                    sMapUnits = "Decimal Degrees";
                    break;
                case esriUnits.esriDecimeters:
                    sMapUnits = "Decimeters";
                    break;
                case esriUnits.esriFeet:
                    sMapUnits = "Feet";
                    break;
                case esriUnits.esriInches:
                    sMapUnits = "Inches";
                    break;
                case esriUnits.esriKilometers:
                    sMapUnits = "Kilometers";
                    break;
                case esriUnits.esriMeters:
                    sMapUnits = "Meters";
                    break;
                case esriUnits.esriMiles:
                    sMapUnits = "Miles";
                    break;
                case esriUnits.esriMillimeters:
                    sMapUnits = "Millimeters";
                    break;
                case esriUnits.esriNauticalMiles:
                    sMapUnits = "NauticalMiles";
                    break;
                case esriUnits.esriPoints:
                    sMapUnits = "Points";
                    break;
                case esriUnits.esriYards:
                    sMapUnits = "Yards";
                    break;
                case esriUnits.esriUnitsLast:
                    sMapUnits = "Unknown";
                    break;

            }
        }
        #region use Symbol selector, need ArcMap installed
        //private void axTOCControl1_OnDoubleClick(object sender, ITOCControlEvents_OnDoubleClickEvent e)
        //{
        //    esriTOCControlItem toccItem = esriTOCControlItem.esriTOCControlItemNone;
        //    ILayer iLayer = null;
        //    IBasicMap iBasicMap = null;
        //    object unk = null;
        //    object data = null;
        //    if(e.button ==1)
        //    {
        //        axTOCControl1.HitTest(e.x, e.y, ref toccItem, ref iBasicMap, ref iLayer, ref unk, ref data);
        //        System.Drawing.Point pos = new System.Drawing.Point(e.x, e.y);
        //        if (toccItem==esriTOCControlItem.esriTOCControlItemLegendClass)
        //        {
        //            ESRI.ArcGIS.Carto.ILegendClass pLC = new LegendClassClass();
        //            ESRI.ArcGIS.Carto.ILegendGroup pLG = new LegendGroupClass();
        //            if (unk is ILegendGroup)
        //            {
        //                pLG = (ILegendGroup)unk;
        //            }
        //            pLC = pLG.get_Class((int)data);
        //            ISymbol pSym;
        //            pSym = pLC.Symbol;
        //            ESRI.ArcGIS.DisplayUI.ISymbolSelector pSS = new ESRI.ArcGIS.DisplayUI.SymbolSelectorClass();
        //            bool bOK = false;
        //            pSS.AddSymbol(pSym);
        //            bOK = pSS.SelectSymbol(0);
        //            if (bOK)
        //            {
        //                pLC.Symbol = pSS.GetSymbolAt(0);
        //            }
        //            this.axMapControl1.ActiveView.Refresh();
        //            this.axTOCControl1.Refresh();
        //        }
        //    }
        //}
        #endregion
        private void axTOCControl1_OnDoubleClick(object sender, ITOCControlEvents_OnDoubleClickEvent e)
        {
            esriTOCControlItem itemType = esriTOCControlItem.esriTOCControlItemNone;
            IBasicMap basicMap = null;
            ILayer layer = null;
            object unk = null;
            object data = null;
            axTOCControl1.HitTest(e.x, e.y, ref itemType, ref basicMap, ref layer, ref unk, ref data);
            if (e.button==1)
            {
                if(itemType==esriTOCControlItem.esriTOCControlItemLegendClass)
                {
                    //get legend
                    ILegendClass pLegendClass = ((ILegendGroup)unk).get_Class((int)data);
                    //create a instance for symbolSelectorfrm
                    SymbolSelectorFrm SymbolSelectorFrm = new SymbolSelectorFrm(pLegendClass, layer);
                    if (SymbolSelectorFrm.ShowDialog()==DialogResult.OK)
                    {
                        //partial update map control
                        m_mapControl.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
                        //set new legend
                        pLegendClass.Symbol = SymbolSelectorFrm.pSymbol;
                        //update map control, toccontrol
                        this.axMapControl1.ActiveView.Refresh();
                        this.axTOCControl1.Refresh();
                    }
                }
            }



        }
    }
}
