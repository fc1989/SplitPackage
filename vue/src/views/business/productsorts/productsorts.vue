<template>
    <simplePage ref="simplepage"
        :title="title" 
        :columnsetting="columnsetting" 
        :rule="rule"
        showSearchFilter
        :searchPage="getPage"
        :getEditModel="getEditModel"
        :defaultSorting="'Id'">
        <template slot="search" slot-scope="slotProps">
          <Input v-model="slotProps.searchData.sortName" :maxlength="50" :placeholder="$t('ProductSorts.SortName')" style="width:150px"></Input>
          <Input v-model="slotProps.searchData.className" :maxlength="50" :placeholder="$t('ProductClasses.ClassName')" style="width:150px"></Input>
          <Input v-model="slotProps.searchData.ptid" :maxlength="50" :placeholder="'PTId'" style="width:150px"></Input>
        </template>
        <template slot="modalForm" slot-scope="slotProps">
            <FormItem :label="$t('ProductSorts.SortName')" prop="sortName">
                <Input v-model="slotProps.model.sortName" :maxlength="50" :minlength="1"></Input>
            </FormItem>
            <FormItem v-if="isedit">
                <Checkbox v-model="slotProps.model.isActive">{{$t('Public.IsActive')}}</Checkbox>
            </FormItem>
        </template>
    </simplePage>
</template>
<script>
import simplePage from "../../../components/simplepagev1.vue";
import productclasses from './productclasses';
import ProductSortApi from "@/api/productsort";

export default {
  components: {
    simplePage,
    productclasses
  },
  methods:{
    async getPage(filter){
        var rep = await ProductSortApi.Search(filter);
        return rep.data.result;
    },
    getEditModel(row){
      this.oldSortName = row.sortName;
      return row;
    }
  },
  data() {
    var _this = this;
    const validateSortName = (rule, value, callback) => {
      if (!value) {
        callback(new Error("sortName is required"));
      } else {
        if(_this.isedit && _this.oldSortName == value){
          callback();
          return;
        }
        ProductSortApi.Verify(value).then(function(rep) {
          if (rep.data.result) {
            callback();
          } else {
            callback("sortName is exit");
          }
        });
      }
    };
    return {
      title: "Menu.Pages.ProductSorts",
      rule: {
          sortName: [{ required: true, validator: validateSortName }]
      },
      columnsetting: {
        actionOption: {
            edit: true,
            delete: true
        },
        columns: [
          {
              type: 'expand',
              width: 20,
              render: (h, params) => {
                  return h(productclasses, {
                      props: {
                          productSortId: params.row.id,
                          productSortName: params.row.sortName
                      }
                  })
              }
          },
          {
            title: this.$t("ProductSorts.SortName"),
            key: "sortName",
            sortable: 'custom'
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
      isedit: null,
      oldSortName: null
    };
  },
  mounted(){
      var _this = this;
      this.$on('on-deleteRow',(id,callback) => {
        ProductSortApi.Delete(id).then(()=>{
            callback();
        });
      });
      this.$on('on-createRow',(model,callback) =>{
        ProductSortApi.Create(model).then(()=>{
            callback();
        });
      });
      this.$on('on-editRow',(model,callback) =>{
        ProductSortApi.Update(model).then(()=>{
            callback();
        });
      });
      this.$on('set-modalState',state => {
        _this.isedit = state === 'edit';
      });
  }
};
</script>