using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MKH
{
    [CreateAssetMenu(fileName = "Ring_", menuName = "Add Item/Equipment_Ring")]
    public class RingData : Item_Equipment
    {
        public override Item Create()
        {
            List<Dictionary<string, object>> data = CSVReader.Read("EquipCSV - Ring");

            Item_Equipment createitem = Instantiate(this);

            if (ItemType.Ring == Type)
            {
                // 장비 등급을 숫자로 변환
                // Normal : 0
                // Magic : 1
                // Rare : 2
                int rate = (int)Rate;

                // 장비 등급 숫자로 주스텟과 설명 추가
                // 주스텟
                createitem.mEffect.Damage = Random.Range((int)data[rate]["최소"], (int)data[rate]["공격력"]);
                // 설명
                createitem.Name = (string)data[rate]["이름"];
                createitem.Description = $"공격력 + {createitem.mEffect.Damage.ToString()}";

                // 부스텟을 모두 0으로 변경
                createitem.mEffect.HP = 0;
                createitem.mEffect.Defense = 0;
                createitem.mEffect.Critical = 0;
                createitem.mEffect.AttackSpeed = 0;
                createitem.mEffect.Stemina = 0;
                createitem.mEffect.EquipRate = 0;
                createitem.mEffect.Speed = 0;
                createitem.mEffect.Mana = 0;

                // 장비 등급 숫자만큼 랜덤한 부스텟 추가
                List<int> list = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7 };
                for (int i = 0; i < rate; i++)  // Noraml : 0회 반복, Magic : 1회 반복, Rare : 2회 반복
                {
                    int select = list[Random.Range(0, list.Count)];
                    list.Remove(select);

                    switch (select)
                    {
                        case 0:
                            createitem.mEffect.HP = (int)data[rate]["체력"];
                            createitem.Description += $"\n체력 + {createitem.mEffect.HP.ToString()}";
                            break;
                        case 1:
                            createitem.mEffect.Defense = (int)data[rate]["방어력"];
                            createitem.Description += $"\n방어력 + {createitem.mEffect.Defense.ToString()}";
                            break;
                        case 2:
                            createitem.mEffect.Critical = (int)data[rate]["치확"];
                            createitem.Description += $"\n치명타 확률 + {createitem.mEffect.Critical.ToString()}%";
                            break;
                        case 3:
                            createitem.mEffect.AttackSpeed = (float)data[rate]["공속"];
                            createitem.Description += $"\n공격속도 + {(createitem.mEffect.AttackSpeed * 100f).ToString()}%";
                            break;
                        case 4:
                            createitem.mEffect.Stemina = (int)data[rate]["스테미나"];
                            createitem.Description += $"\n스테미나 + {createitem.mEffect.Stemina.ToString()}";
                            break;
                        case 5:
                            createitem.mEffect.EquipRate = (float)data[rate]["장비획득률 증가"];
                            createitem.Description += $"\n장비 획득률 + {(createitem.mEffect.EquipRate * 100f).ToString()}%";
                            break;
                        case 6:
                            createitem.mEffect.Speed = (float)data[rate]["이속"];
                            createitem.Description += $"\n이동속도 + {(createitem.mEffect.Speed * 100f).ToString()}%";
                            break;
                        case 7:
                            createitem.mEffect.Mana = (int)data[rate]["마나"];
                            createitem.Description += $"\n마나 + {createitem.mEffect.Mana.ToString()}";
                            break;
                    }

                }
            }
            return createitem;
        }
    }
}
