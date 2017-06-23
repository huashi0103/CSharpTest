using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Interop.Word;
using MSWord = Microsoft.Office.Interop.Word;
using System.Text.RegularExpressions;

namespace CSharpTest
{
    public  class CSWord
    {
        private Application wordApp = null;
        private Document wordDoc = null;
        private string docPath = null;
        private object Nothing = System.Reflection.Missing.Value;
        public  CSWord(string path,bool isOpen)
        {
            try
            {
                 docPath = path;
                Object Nothing = System.Reflection.Missing.Value;
                wordApp = new MSWord.Application();
                wordApp.Visible = false;
               
                wordApp.DisplayAlerts = WdAlertLevel.wdAlertsNone;
                //创建Word文档
                if (isOpen)
                {

                    wordDoc = wordApp.Documents.Open(path);
                }
                else
                {
                    wordDoc = wordApp.Documents.Add(ref Nothing, ref Nothing, ref Nothing, ref Nothing);
                }
                ((DocumentEvents2_Event)wordDoc).Close += () => {
                    Console.WriteLine("Close");
                };
                ((ApplicationEvents4_Event)wordApp).Quit+= () =>
                {
                    Console.WriteLine("quit");
                };
            }
            catch(Exception ex) {

                throw new Exception("创建word对象失败,error:"+ex.Message);
            }
        }


        public void SetFont()
        {
            wordApp.Selection.Font.Bold = 0;
            wordApp.Selection.Font.Italic = 0;
            wordApp.Selection.Font.Subscript = 0;

         
        }
        public void SetFontName(string strType)
        {
            wordApp.Selection.Font.Name = strType;

        }
        public void SetFontSize(int nSize)
        {
            wordApp.Selection.Font.Size = nSize;
        }
        public void SetAlignment(WdParagraphAlignment align)
        {
            wordApp.Selection.ParagraphFormat.Alignment = align;
        }
        public void GoToTheEnd()
        {
            object unit;
            unit = Microsoft.Office.Interop.Word.WdUnits.wdStory;
            wordApp.Selection.EndKey(ref   unit, ref   Nothing);

        }
        public void GoToTheBeginning()
        {
            object unit;
            unit = Microsoft.Office.Interop.Word.WdUnits.wdStory;
            wordApp.Selection.HomeKey(ref   unit, ref   Nothing);

        }
        
        /// <summary>
        /// 添加文字
        /// </summary>
        /// <param name="txtContent"></param>
        /// <param name="Bold"></param>
        /// <param name="size"></param>
        public void AddText(string txtContent,int Bold=0,int size=14)
        {
            //wordApp.Selection.Font.Spacing = 10;//字符间隔

            wordApp.Selection.Font.Bold = Bold;
            wordApp.Selection.Font.Size = size;
            wordApp.Selection.TypeText(txtContent);    
            wordApp.Selection.TypeParagraph();
        }
        /// <summary>
        /// 添加图片
        /// </summary>
        /// <param name="filepath"></param>
        public void AddPic(string filepath)
        {
            object range = wordDoc.Paragraphs.Last.Range;
            //定义该图片是否为外s部链接
            object linkToFile = false;//默认
            //定义插入的图片是否随word一起保存
            object saveWithDocument = true;
            //向word中写入图片
            wordDoc.InlineShapes.AddPicture(filepath, ref Nothing, ref Nothing, ref Nothing);

            object unite = Microsoft.Office.Interop.Word.WdUnits.wdStory;
            wordApp.Selection.ParagraphFormat.Alignment = MSWord.WdParagraphAlignment.wdAlignParagraphCenter;//居中显示图片
            //wordDoc.InlineShapes[1].Height = 130;
            //wordDoc.InlineShapes[1].Width = 200;
            wordDoc.Content.InsertAfter("\n");
            wordApp.Selection.EndKey(ref unite, ref Nothing);
            wordApp.Selection.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
            wordApp.Selection.Font.Size = 10;//字体大小
            wordApp.Selection.TypeText("图1 测试图片\n");

        }
        /// <summary>
        /// 添加表
        /// </summary>
        public void AddTable()
        {

            int tableRow = 6;
            int tableColumn = 6;
            //定义一个word中的表格对象
            MSWord.Table table = wordDoc.Tables.Add(wordApp.Selection.Range, tableRow, tableColumn, ref Nothing, ref Nothing);
            wordDoc.Tables[1].Cell(1, 1).Range.Text = "列\n行";
            for (int i = 1; i < tableRow; i++)
            {
                for (int j = 1; j < tableColumn; j++)
                {
                    if (i == 1)
                    {
                        table.Cell(i, j + 1).Range.Text = "Column " + j;
                    }
                    if (j == 1)
                    {
                        table.Cell(i + 1, j).Range.Text = "Row " + i;
                    }
                    table.Cell(i + 1, j + 1).Range.Text = i + "行 " + j + "列";
                }
            }


            //添加行
            table.Rows.Add(ref Nothing);
            table.Rows[tableRow + 1].Height = 45;
            //向新添加的行的单元格中添加图片
            //string FileName = "d:\\kk.jpg";//图片所在路径
            object LinkToFile = false;
            object SaveWithDocument = true;
            object Anchor = table.Cell(tableRow + 1, tableColumn).Range;//选中要添加图片的单元格
            //wordDoc.Application.ActiveDocument.InlineShapes.AddPicture(FileName, ref LinkToFile, ref SaveWithDocument, ref Anchor);
            //wordDoc.Application.ActiveDocument.InlineShapes[1].Width = 75;//图片宽度
            //wordDoc.Application.ActiveDocument.InlineShapes[1].Height = 45;//图片高度
            // 将图片设置为四周环绕型
            //MSWord.Shape s = wordDoc.Application.ActiveDocument.InlineShapes[1].ConvertToShape();
            //s.WrapFormat.Type = MSWord.WdWrapType.wdWrapSquare;
            //设置table样式
            table.Rows.HeightRule = MSWord.WdRowHeightRule.wdRowHeightAtLeast;
            table.Rows.Height = wordApp.CentimetersToPoints(float.Parse("0.8"));

            table.Range.Font.Size = 10.5F;
            table.Range.Font.Bold = 0;

            table.Range.ParagraphFormat.Alignment = MSWord.WdParagraphAlignment.wdAlignParagraphCenter;
            table.Range.Cells.VerticalAlignment = MSWord.WdCellVerticalAlignment.wdCellAlignVerticalBottom;
            //设置table边框样式
            table.Borders.OutsideLineStyle = MSWord.WdLineStyle.wdLineStyleDouble;
            table.Borders.InsideLineStyle = MSWord.WdLineStyle.wdLineStyleSingle;

            table.Rows[1].Range.Font.Bold = 1;
            table.Rows[1].Range.Font.Size = 12F;
            table.Cell(1, 1).Range.Font.Size = 10.5F;
            wordApp.Selection.Cells.Height = 40;//所有单元格的高度
            for (int i = 2; i <= tableRow; i++)
            {
                table.Rows[i].Height = 20;
            }
            table.Cell(1, 1).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphRight;
            table.Cell(1, 1).Range.Paragraphs[2].Format.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphLeft;

            table.Columns[1].Width = 50;
            for (int i = 2; i <= tableColumn; i++)
            {
                table.Columns[i].Width = 75;
            }


            //添加表头斜线,并设置表头的样式
            table.Cell(1, 1).Borders[Microsoft.Office.Interop.Word.WdBorderType.wdBorderDiagonalDown].Visible = true;
            table.Cell(1, 1).Borders[Microsoft.Office.Interop.Word.WdBorderType.wdBorderDiagonalDown].Color = Microsoft.Office.Interop.Word.WdColor.wdColorGray60;
            table.Cell(1, 1).Borders[Microsoft.Office.Interop.Word.WdBorderType.wdBorderDiagonalDown].LineWidth = Microsoft.Office.Interop.Word.WdLineWidth.wdLineWidth050pt;

            //表格边框
            //表格内容行边框
            SetTableBorderStyle(table, MSWord.WdBorderType.wdBorderHorizontal, MSWord.WdColor.wdColorGray20, MSWord.WdLineWidth.wdLineWidth025pt);
            //表格内容列边框
            SetTableBorderStyle(table, MSWord.WdBorderType.wdBorderVertical, MSWord.WdColor.wdColorGray20, MSWord.WdLineWidth.wdLineWidth025pt);
            SetTableBorderStyle(table, MSWord.WdBorderType.wdBorderLeft, MSWord.WdColor.wdColorGray50, MSWord.WdLineWidth.wdLineWidth050pt);
            SetTableBorderStyle(table, MSWord.WdBorderType.wdBorderRight, MSWord.WdColor.wdColorGray50, MSWord.WdLineWidth.wdLineWidth050pt);
            SetTableBorderStyle(table, MSWord.WdBorderType.wdBorderTop, MSWord.WdColor.wdColorGray50, MSWord.WdLineWidth.wdLineWidth050pt);
            SetTableBorderStyle(table, MSWord.WdBorderType.wdBorderBottom, MSWord.WdColor.wdColorGray50, MSWord.WdLineWidth.wdLineWidth050pt);
            //合并单元格
            table.Cell(4, 4).Merge(table.Cell(4, 5));//横向合并
            table.Cell(2, 3).Merge(table.Cell(4, 3));//纵向合并

        }
        public void OpenModel(string modelPath)
        {
            object ModelName = modelPath;
            wordDoc = wordApp.Documents.Open(ref ModelName);//打开word模板
            //下面操作模板。。。。

        }

        private void SetTableBorderStyle(Table table1,MSWord.WdBorderType bdType,MSWord.WdColor color,MSWord.WdLineWidth width)
        {
            table1.Borders[bdType].Visible = true;
            table1.Borders[bdType].Color = color;
            table1.Borders[bdType].LineWidth = width;
        }
       /// <summary>
       /// 保存
       /// </summary>
       /// <returns></returns>
        public bool Save()
        {
            try
            {
                object path = docPath;
                object format = MSWord.WdSaveFormat.wdFormatDocument;
  
                wordDoc.SaveAs(ref path, ref format);
                wordDoc.Close(ref Nothing, ref Nothing, ref Nothing);
                wordApp.Quit(ref Nothing, ref Nothing, ref Nothing);
                return true;
            }
            catch
            {
                return false;
            }
        }
       /// <summary>
       /// 替换
       /// </summary>
       /// <param name="strOldText"></param>
       /// <param name="strNewText"></param>
        public void Replace(string strOldText, string strNewText)
        {
      
            this.wordApp.Selection.Find.ClearFormatting();//移除Find的搜索文本和段落格式设置  
            wordApp.Selection.Find.Text = strOldText;//需要查找的字符
            if (wordApp.Selection.Find.Execute())//查找字符
            {
                wordApp.Selection.TypeText(strNewText);//在找到的字符区域写数据
            }


        }

        /// <summary>  
        ///查找字符 
        /// </summary>  
        /// <returns></returns>  
        public bool  FindStr(string str)
        {
            Find find = wordApp.Selection.Find;
            Object findText;
            Object matchCase = Type.Missing;
            Object matchWholeWord = Type.Missing;
            Object matchWildcards = Type.Missing;
            Object matchSoundsLike = Type.Missing;
            Object matchAllWordForms = Type.Missing;
            Object forward = true;
            Object wrap = WdFindWrap.wdFindStop;
            Object format = Type.Missing;
            Object replaceWith = Type.Missing;
            Object replace = Type.Missing;
            Object matchKashida = Type.Missing;
            Object matchDiacritics = Type.Missing;
            Object matchAlefHamza = Type.Missing;
            Object matchControl = Type.Missing;

            if ((str == "") || (str == string.Empty))
                findText = find.Text;
            else
                findText = str;
            find.ClearFormatting();
            return find.Execute(ref findText, ref matchCase, ref matchWholeWord,
                ref matchWildcards, ref matchSoundsLike, ref matchAllWordForms,
                ref forward, ref wrap, ref format, ref replaceWith, ref replace,
                ref matchKashida, ref matchDiacritics, ref matchAlefHamza,
                ref matchControl);  
        }  

        /// <summary>  
        /// 保存成HTML  
        /// </summary>  
        /// <param name="strFileName"></param>  
        public void SaveAsHtml(string strFileName)
        {
            Type wordType = wordApp.GetType();
            object missing = System.Reflection.Missing.Value;
            object fileName = strFileName;
            object Format = (int)Microsoft.Office.Interop.Word.WdSaveFormat.wdFormatHTML;
            wordDoc.SaveAs(ref   fileName, ref   Format, ref   missing, ref   missing, ref   missing, ref   missing, ref   missing,
                    ref   missing, ref   missing, ref   missing, ref   missing, ref   missing, ref   missing, ref   missing, ref   missing, ref   missing);


        }

        public void Dispose()
        {
            if (wordDoc != null)
            {
                ((_Document)wordDoc).Close();
            }
            if (wordApp != null)
            {
                ((_Application)wordApp).Quit();
            }
        }


        public List<MCU> test()
        {
            List<MCU> mculist = new List<MCU>();
            Paragraphs pgs = wordApp.Selection.Paragraphs;
            foreach (Paragraph pg in pgs)
            {
                foreach (Table table in wordDoc.Tables)
                {
                    int k = 0;
                    MCU mcu = new MCU();
                    foreach (Row row in table.Rows)
                    {
                        string temp = "";
                         foreach(Cell cell in row.Cells)
                        {
                            string s = cell.Range.Text.Trim();
                            foreach (char cha in s)
                            {
                                if(cha!=7&&cha!=13)
                                    temp+=cha;
                            }
                            if(cell.Next!=null)temp += ',';
                        }
                        //Console.WriteLine(temp);
                         if (k == 0) mcu.mcustation = temp.Replace(",","");
                         else if (k == 1) mcu.IP = temp.Split(':')[1].Replace(",", "");
                         else if (k == 2) { k++; continue; }
                         else
                         {
                             if (temp.Contains(','))
                             {
                                 var str = temp.Split(',');
                                 if (!String.IsNullOrEmpty(str[0])) mcu.SurveyPoint.Add(str[0], str[1]);
                                 if (!String.IsNullOrEmpty(str[2])) mcu.SurveyPoint.Add(str[2], str[3]);
                             }
                         }
                        k++;
                    }
                    mculist.Add(mcu);
                }
            }
            return mculist;
            //foreach (var m in mculist)
            //{
            //    Console.Write(m.ToString());
            //    Console.WriteLine();
            //}
        }
    }
    public class MCU
    {
        public string mcustation;
        public string IP;
        public Dictionary<string, string> SurveyPoint = new Dictionary<string, string>();
        public override string ToString()
        {
            string s = "测站:" + mcustation + Environment.NewLine;
            s += "IP:" + IP + Environment.NewLine;
            foreach (var dic in SurveyPoint)
            {
                s += "通道:" + dic.Key + "," + "测点:" + dic.Value + Environment.NewLine;
            }
            return s;
        }
    }
   
}

