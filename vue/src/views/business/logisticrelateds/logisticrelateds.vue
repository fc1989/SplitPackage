<template>
    <simplePage ref="simplepage" 
        :title="title" 
        :columnsetting="columnsetting" 
        :rule="rule"
        :getCreateModel="getCreateModel"
        :getEditModel="getEditModel"
        :searchPage="getPage">
        <template slot="modalForm" slot-scope="slotProps">
            <FormItem :label="$t('LogisticRelateds.RelatedName')" prop="relatedName">
                <Input v-model="slotProps.model.relatedName" :maxlength="200" :minlength="1"></Input>
            </FormItem>
            <Transfer
                :data="transferState.logistics"
                :target-keys="slotProps.model.logisticIds"
                filterable
                :filter-method="filterLogistic"
                @on-change="handleChange">
            </Transfer>
        </template>
    </simplePage>
</template>

<script>
import simplePage from "../../../components/simplepagev1.vue";
import LogisticRelatedApi from "@/api/logisticrelated";
import LogisticApi from "@/api/logistic";
import logisticrelateditem from "./logisticrelateditem";

export default {
  components: {
    simplePage,
    logisticrelateditem
  },
  methods:{
    async getPage(filter){
        var rep = await LogisticRelatedApi.Search(filter);
        return rep.data.result;
    },
    async getCreateModel(){
        var _this = this;
        return {
            relatedName: null,
            logisticIds:[]
        };
    },
    async getEditModel(row){
        return {
            id: row.id,
            relatedName: row.relatedName,
            logisticIds: row.items.map(function(vl, index, arr){
                return vl.logisticId;
            })
        };
    },
    filterLogistic(data, query){
        var isMatch = data.label.indexOf(query) > -1;
        if(!isMatch){
            data.disabled = true;
        }
        else{
            data.disabled = false;
        }
        return isMatch;
    },
    handleChange(newTargetKeys, direction, moveKeys){
        this.$refs.simplepage.modalState.model.logisticIds = newTargetKeys;
    }
  },
  data() {
    return {
      title: "Menu.Pages.LogisticRelateds",
      rule: {
          relatedName: [{ required: true }]
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
                  return h(logisticrelateditem, {
                      props: {
                          logisticRelated: params.row
                      }
                  })
              }
          },
          {
            title: this.$t("LogisticRelateds.RelatedName"),
            key: "relatedName"
          }
        ]
      },
      transferState: {
          logistics: []
      }
    };
  },
  mounted(){
      this.$on('on-deleteRow',(id,callback) => {
        LogisticRelatedApi.Delete(id).then(()=>{
            callback();
        });
      });
      this.$on('on-createRow',(model,callback) =>{
        LogisticRelatedApi.Create(model).then(()=>{
            callback();
        });
      });
      this.$on('on-editRow',(model,callback) =>{
        LogisticRelatedApi.Update(model).then(()=>{
            callback();
        });
      });
  },
  async created(){
    var _this = this;
    LogisticApi.Search({
        maxResultCount: 10000
    }).then(req => {
        _this.transferState.logistics = req.data.result.items.map(function(vl, index, arr){
            return {
                key: vl.id,
                label: vl.corporationName,
                disabled: false
            };
        });
    });
  }
}
</script>
