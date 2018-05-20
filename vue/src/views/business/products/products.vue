<template>
    <simplePage ref="simplepage"
        :title="title" 
        :columnsetting="columnsetting" 
        :rule="rule"
        showSearchFilter
        :getCreateModel="getCreateModel"
        :getEditModel="getEditModel"
        :searchPage="getPage">
        <template slot="search" slot-scope="slotProps">
            <Input v-model="slotProps.searchData.productName" :maxlength="50" :placeholder="$t('Products.ProductName')" style="width:150px"></Input>
            <Input v-model="slotProps.searchData.sku" :maxlength="50" :placeholder="$t('Products.Sku')" style="width:150px"></Input>
            <Input v-model="slotProps.searchData.brand" :maxlength="50" :placeholder="$t('Products.Brand')" style="width:150px"></Input>
            <Input v-model="slotProps.searchData.ptid" :maxlength="50" :placeholder="$t('Menu.Pages.ProductClasses')" style="width:150px"></Input>
        </template>
        <template slot="modalForm" slot-scope="slotProps">
            <FormItem :label="$t('Products.ProductName')" prop="productName">
                <Input v-model="slotProps.model.productName" :maxlength="200" :minlength="1"></Input>
            </FormItem>
            <FormItem :label="$t('Products.Sku')" prop="sku">
                <Input v-model="slotProps.model.sku" :maxlength="50" :disabled="showSpecified"></Input>
            </FormItem>
            <FormItem :label="$t('Menu.Pages.ProductClasses')" prop="ptid">
                <Cascader :data="cascaderData" v-model="cascaderValue"></Cascader>
            </FormItem>
            <FormItem :label="$t('Products.Brand')" prop="brand">
                <Input v-model="slotProps.model.brand" :maxlength="50"></Input>
            </FormItem>
            <FormItem :label="$t('Products.Weight')" prop="weight">
                <Input-number v-model.number="slotProps.model.weight" style="width:100%"></Input-number>
            </FormItem>
            <FormItem :label="$t('Products.DeclarePrice')" prop="declarePrice">
                <Input-number v-model.number="slotProps.model.declarePrice" style="width:100%"></Input-number>
            </FormItem>
            <FormItem :label="$t('Products.DeclareTaxrate')" prop="declareTaxrate">
                <Input-number v-model.number="slotProps.model.declareTaxrate" style="width:100%"></Input-number>
            </FormItem>
        </template>
    </simplePage>
</template>
<script>
import simplePage from "../../../components/simplepagev1.vue";
import ProductClassApi from "@/api/productclass";
import ProductApi from "@/api/product";

export default {
  components: {
    simplePage
  },

  methods:{
    async getPage(filter){
        var rep = await ProductApi.Search(filter);
        return rep.data.result;
    },
    getCreateModel(){
      this.cascaderValue = [];
      return {
        weight: 0,
        declarePrice: 0,
        declareTaxrate: 0
      };
    },
    getEditModel(row){
      var productSortId = "";
      var ptid = row.ptid;
      for(var item in this.cascaderData){
          var array1 = this.cascaderData[item].children.filter(vl => {
              return vl.value === ptid;
          });
          if(array1.length > 0)
          {
              productSortId = this.cascaderData[item].value;
              break;          
          }
      }
      this.cascaderValue = [productSortId,ptid];
      return row;
    }
  },
  data() {
    var _this = this;
    const validateProductClass = (rule, value, callback) => {
        if (_this.cascaderValue.length < 1) {
            callback(new Error("ProductClass is required"));
        }
        callback();
    };
    const validateSku = (rule, value, callback) => {
      if (!value) {
        callback(new Error("sku is required"));
      } else {
        if(_this.showSpecified)
        {
          callback();
          return;
        }
        ProductApi.Verify(value).then(function(rep) {
          if (rep.data.result) {
            callback();
          } else {
            callback("sku is exit");
          }
        });
      }
    };
    return {
      title: "Menu.Pages.Products",
      rule: {
        productName: [{ required: true }],
        sku: [{ required: true, validator: validateSku }],
        weight: [{ type: "number" }],
        ptid: [{required: true,validator: validateProductClass}],
        declarePrice: [{ type: "number" }],
        declareTaxrate: [{ type: "number" }]
      },
      columnsetting: {
        actionOption: {
            edit: true,
            delete: true
        },
        columns: [
          {
            title: this.$t("Products.ProductName"),
            key: "productName"
          },
          {
            title: this.$t("Products.Sku"),
            key: "sku"
          },
          {
            title: this.$t("Products.Brand"),
            key: "brand"
          },
          {
            title: this.$t("Products.Weight"),
            key: "weight"
          },
          {
            title: this.$t("Products.DeclarePrice"),
            key: "declarePrice"
          },
          {
            title: this.$t("Products.DeclareTaxrate"),
            key: "declareTaxrate"
          },
          {
            title: this.$t("Menu.Pages.ProductClasses"),
            key: "ptid"
          },
          {
            title: this.$t("Public.IsActive"),
            render: (h, params) => {
              return h("Checkbox", {
                props: {
                  value: params.row.isActive,
                  disabled: true
                }
              });
            }
          }
        ]
      },
      showSpecified: null,
      cascaderData: [],
      cascaderValue: []
    };
  },
  mounted(){
      var _this = this;
      this.$on('on-deleteRow',(id,callback) => {
        ProductApi.Delete(id).then(()=>{
            callback();
        });
      });
      this.$on('on-createRow',(model,callback) =>{
        model.ptid = _this.cascaderValue[1];
        ProductApi.Create(model).then(()=>{
            callback();
        });
      });
      this.$on('on-editRow',(model,callback) =>{
        model.ptid = _this.cascaderValue[1];
        ProductApi.Update(model).then(()=>{
            callback();
        });
      });
      this.$on('set-modalState',state => {
        _this.showSpecified = state === 'edit';
      });
  },
  created() {
      var _this = this;
      ProductClassApi.GetOptional().then(req => {
          _this.cascaderData = req.data.result.map(function(vl, index, arr){
              return {
                  value: vl.value,
                  label: vl.label,
                  children: vl.children.map(function(vl1, index1, arr1){
                      return {
                      value: vl1.value,
                      label: vl1.label,
                      };
                  })
              };
          });
      });
  }
};
</script>