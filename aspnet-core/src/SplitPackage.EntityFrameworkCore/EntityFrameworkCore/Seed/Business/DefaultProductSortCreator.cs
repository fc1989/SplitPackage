using Microsoft.EntityFrameworkCore;
using SplitPackage.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SplitPackage.EntityFrameworkCore.Seed.Business
{
    public class DefaultProductSortCreator
    {
        private readonly SplitPackageDbContext _context;

        public DefaultProductSortCreator(SplitPackageDbContext context)
        {
            this._context = context;
        }

        public void Create()
        {
            if (_context.ProductSort.Any())
            {
                return;
            }
            var productSorts = new Dictionary<string, List<Tuple<string, string>>>() {
                {
                    "奶粉",
                    new List<Tuple<string, string>>(){
                        Tuple.Create("婴儿奶粉(1—2段)","1010703"),
                        Tuple.Create("婴儿奶粉(3—4段)","1010704"),
                        Tuple.Create("儿童奶粉(袋装)","1010702"),
                        Tuple.Create("儿童奶粉(罐装)","1010707"),
                        Tuple.Create("成人奶粉(袋装)","1010701"),
                        Tuple.Create("成人奶粉(罐装)","1010706"),
                        Tuple.Create("羊奶粉(罐装)","101070601"),
                        Tuple.Create("孕妇奶粉(罐装)","101070602")
                    }
                },
                {
                    "保健品",
                    new List<Tuple<string,string>>(){
                        Tuple.Create("elevit爱乐维系列","101990401"),
                        Tuple.Create("Unichi系列","101990404"),
                        Tuple.Create("USANA系列","101990406"),
                        Tuple.Create("益生菌","101990417"),
                        Tuple.Create("辅酶","101990409"),
                        Tuple.Create("减肥粉","9020222"),
                        Tuple.Create("其它保健品","1019904")
                    }
                },
                {
                    "洗护用品",
                    new List<Tuple<string,string>>(){
                        Tuple.Create("低价护肤品(单价低于$10)","9020200"),
                        Tuple.Create("精华素","9020210"),
                        Tuple.Create("化妆水","9020211"),
                        Tuple.Create("Jurlique茱莉蔻系列","9020217"),
                        Tuple.Create("Aesop伊索系列","9020218"),
                        Tuple.Create("Freezeframe系列","9020220"),
                        Tuple.Create("羊胎素精华","9020216"),
                        Tuple.Create("羊胎素礼盒","101990418"),
                        Tuple.Create("其它高价护肤品(单价高于$10)","9020209"),
                        Tuple.Create("其它护肤品套装","9020213"),
                        Tuple.Create("沐浴露","9020101"),
                        Tuple.Create("洗发水","9020102"),
                        Tuple.Create("护发用品","9020108"),
                        Tuple.Create("洗洁精","9020104"),
                        Tuple.Create("洗衣粉","9020105"),
                        Tuple.Create("香皂","9020107"),
                        Tuple.Create("其它洗液用品","9020100")
                    }
                },
                {
                    "化妆品",
                    new List<Tuple<string,string>>(){
                        Tuple.Create("雅诗兰黛系列","9010001"),
                        Tuple.Create("唇用化妆品","9010200"),
                        Tuple.Create("其它化妆品","9010000"),
                        Tuple.Create("化妆品套装","9010300")
                    }
                },
                {
                    "居家日用",
                    new List<Tuple<string,string>>(){
                        Tuple.Create("儿童牙刷牙膏","9029901"),
                        Tuple.Create("成人牙膏","9029902"),
                        Tuple.Create("电动牙刷","9029914"),
                        Tuple.Create("瘦脸仪","9029923"),
                        Tuple.Create("洗脸仪","9020001"),
                        Tuple.Create("酸奶机","9029922"),
                        Tuple.Create("Mixgo果汁机","9029915"),
                        Tuple.Create("手动吸奶器","9029916"),
                        Tuple.Create("奶瓶消毒器","9029917"),
                        Tuple.Create("空气净化器","9029918"),
                        Tuple.Create("香薰机","9029919"),
                        Tuple.Create("其它小家电","9029913"),
                        Tuple.Create("卫生巾","9029905"),
                        Tuple.Create("尿不湿","9029906"),
                        Tuple.Create("b box系列","9029904"),
                        Tuple.Create("奶嘴","9029907"),
                        Tuple.Create("婴儿水杯","9029907"),
                        Tuple.Create("成人水杯","9029921"),
                        Tuple.Create("保温杯","9029908"),
                        Tuple.Create("香薰蜡烛","9029912"),
                        Tuple.Create("太阳眼镜","9029920"),
                        Tuple.Create("奶瓶(套装)","9029910"),
                        Tuple.Create("其它日用品","9029900"),
                        Tuple.Create("其它日用品(套装)","9029999")
                    }
                },
                {
                    "食品",
                    new List<Tuple<string,string>>(){
                        Tuple.Create("薯片","101990301"),
                        Tuple.Create("麦片","101990301"),
                        Tuple.Create("饼干","101990301"),
                        Tuple.Create("面条","101990301"),
                        Tuple.Create("通心粉","101990301"),
                        Tuple.Create("糖果","101990302"),
                        Tuple.Create("巧克力","101990303"),
                        Tuple.Create("婴儿辅食","101990304"),
                        Tuple.Create("麦卢卡蜂蜜","101990306"),
                        Tuple.Create("COMVITA蜂蜜","101990311"),
                        Tuple.Create("其它蜂蜜","101990305"),
                        Tuple.Create("酸奶粉","101990312"),
                        Tuple.Create("茶包","101990307"),
                        Tuple.Create("咖啡","101990308"),
                        Tuple.Create("水果","101990309"),
                        Tuple.Create("猫粮狗粮","101990415"),
                        Tuple.Create("其它食品","1019903")
                    }
                },
                {
                    "纺织类",
                    new List<Tuple<string,string>>(){
                        Tuple.Create("围巾","4020001"),
                        Tuple.Create("袜子、口罩","4020004"),
                        Tuple.Create("压力袜","4019900"),
                        Tuple.Create("内衣裤","4020002"),
                        Tuple.Create("配饰（帽子、手套）","4020003"),
                        Tuple.Create("配饰（皮带、披肩）","4020005"),
                        Tuple.Create("衣服","4020000"),
                        Tuple.Create("被子","4030001"),
                        Tuple.Create("枕头","4030002")
                    }
                },
                {
                    "鞋",
                    new List<Tuple<string,string>>(){
                        Tuple.Create("其它鞋","6020000"),
                        Tuple.Create("童鞋,拖鞋","6020001")
                    }
                },
                {
                    "玩具",
                    new List<Tuple<string,string>>(){
                        Tuple.Create("JellyCat系列","22020201"),
                        Tuple.Create("其它玩具","22020200")
                    }
                },
                {
                    "酒类",
                    new List<Tuple<string, string>>()
                },
                {
                    "箱包",
                    new List<Tuple<string, string>>()
                },
                {
                    "手表",
                    new List<Tuple<string, string>>()
                }
            };
            foreach (var item in productSorts)
            {
                var pr = new ProductSort()
                {
                    SortName = item.Key
                };
                List<ProductClass> pc = new List<ProductClass>();
                foreach (var ii in item.Value)
                {
                    pc.Add(new ProductClass() {
                        ClassName = ii.Item1,
                        PTId = ii.Item2,
                        ProductSortBy = pr
                    });
                }
                pr.Items = pc;
                _context.ProductSort.Add(pr);
            }
            _context.SaveChanges();
        }
    }
}
