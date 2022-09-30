using NexenHub.Class;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace NexenHub.Models
{
    public class MaterialInUseViewModel
    {
        public List<(InputedMaterial, InputCheck)> CheckedMaterial { get; set; }

        GlobalDatabase dbglob = new GlobalDatabase();

        public MaterialInUseViewModel(string EQ_ID)
        {
            LoadInputedMaterial(EQ_ID);
        }

        public void LoadInputedMaterial(string EQ_ID)
        {
            CheckedMaterial = new List<(InputedMaterial, InputCheck)>();

            DataTable dt = dbglob.GetInputedMaterial(EQ_ID);
            foreach (DataRow row in dt.Rows)
            {
                InputedMaterial material = new InputedMaterial();
                material.EQ_ID = EQ_ID;
                material.LOT_ID = row["LOT_ID"].ToString();
                material.IO_POSID = row["IO_POSID"].ToString();
                material.ITEM_ID = row["ITEM_ID"].ToString();
                material.ITEM_NAME = row["ITEM_NAME"].ToString();
                material.CART_ID = row["CART_ID"].ToString();

                DataRow rowMini = dbglob.Pos2MiniPc(material.IO_POSID, EQ_ID);
                if (rowMini != null)
                {
                    material.MiniPC_ID = rowMini["MINIPC_ID"].ToString();
                }

                InputCheck inputCheck = new InputCheck(material.LOT_ID, EQ_ID, material.MiniPC_ID);

                CheckedMaterial.Add((material, inputCheck));
                
            }

        }


    }
}
