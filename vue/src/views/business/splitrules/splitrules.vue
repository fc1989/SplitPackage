<template>
    <simplePage ref="simplepage"
        :title="title" 
        :columnsetting="columnsetting" 
        :rule="rule"
        showSearchFilter
        :searchPage="getPage"
        :getCreateModel="getCreateModel"
        :getEditModel="getEditModel">
        <template slot="search" slot-scope="slotProps">
          <Input v-model="slotProps.searchData.logisticName" :maxlength="50" :placeholder="$t('Menu.Pages.Logistics')" style="width:150px"></Input>
          <Input v-model="slotProps.searchData.channelName" :maxlength="50" :placeholder="$t('Menu.Pages.LogisticChannels')" style="width:150px"></Input>
          <Input v-model="slotProps.searchData.ptid" :maxlength="50" :placeholder="'PTId'" style="width:150px"></Input>
        </template>
        <template slot="modalForm" slot-scope="slotProps">
          <FormItem :label="$t('Menu.Pages.LogisticChannels')" prop="logisticChannelId">
            <Cascader :data="cascaderData" v-model="cascaderValue" :disabled="showSpecified"></Cascader>
          </FormItem>
          <FormItem :label="$t('SplitRules.RuleName')" prop="ruleName">
              <Input v-model="slotProps.model.ruleName" style="width:100%"></Input>
          </FormItem>
          <FormItem :label="$t('SplitRules.MaxPackage')" prop="maxPackage">
              <Input-number v-model.number="slotProps.model.maxPackage" style="width:100%"></Input-number>
          </FormItem>
          <FormItem :label="$t('SplitRules.MaxWeight') + '(g)'" prop="maxWeight">
              <Input-number v-model.number="slotProps.model.maxWeight" style="width:100%"></Input-number>
          </FormItem>
          <FormItem :label="$t('SplitRules.MaxTax') + '(AUD)'" prop="maxTax">
              <Input-number v-model.number="slotProps.model.maxTax" style="width:100%"></Input-number>
          </FormItem>
          <FormItem :label="$t('SplitRules.MaxPrice') + '(AUD)'" prop="maxPrice">
              <Input-number v-model.number="slotProps.model.maxPrice" style="width:100%"></Input-number>
          </FormItem>
            <FormItem v-if="showSpecified">
                <Checkbox v-model="slotProps.model.isActive" disabled="disabled">{{$t('Public.IsActive')}}</Checkbox>
            </FormItem>
        </template>
    </simplePage>
</template>
<script>
import simplePage from "../../../components/simplepagev1.vue";
import ruleItems from './ruleitems'
import SplitRuleApi from "@/api/splitrule";
import LogisticChannelApi from "@/api/logisticchannel";

export default {
  components: {
    simplePage,
    ruleItems
  },
  methods:{
    async getPage(filter){
        var rep = await SplitRuleApi.Search(filter);
        return rep.data.result;
    },
    getCreateModel(){
      this.cascaderValue = [];
      return {
        maxPackage: 0,
        maxWeight: 0,
        maxTax: 0,
        maxPrice: 0,
        isActive: true
      };
    },
    async getEditModel(row){
      var req = await SplitRuleApi.Get(row.id);
      var result = req.data.result;
      var lId = "";
      var lcId = result.logisticChannelId.toString();
      for(var item in this.cascaderData){
        var array = this.cascaderData[item].children.filter(vl => {
          return vl.value === lcId;
        });
        if(array.length > 0)
        {
          lId = this.cascaderData[item].value.toString();
          break;
        }
      }
      this.cascaderValue = [lId,lcId];
      return result;
    }
  },
  data() {
    var _this = this;
    const validateLogisticChannelId = (rule, value, callback) => {
      if (_this.cascaderValue.length == 0) {
        callback(new Error("logisticChannelId is required"));
      }
      callback();
    };
    return {
      title: "Menu.Pages.SplitRules",
      rule: {
        logisticChannelId: [{ required: true, validator:validateLogisticChannelId, trigger: 'ignore' }],
        maxPackage: [{ required: true, type:'number', trigger: 'ignore' }],
        maxWeight: [{ required: true, type:'number', trigger: 'ignore' }],
        maxTax: [{ required: true, type:'number', trigger: 'ignore' }],
        maxPrice: [{ required: true, type:'number', trigger: 'ignore' }]
      },
      columnsetting: {
        actionOption: {
            edit: function(row, vm) {
              if(row.tenantId === vm.$store.state.session.tenantId){
                return true;
              }
              return false;
            },
            delete: function(row, vm) {
              if(row.tenantId === vm.$store.state.session.tenantId){
                return true;
              }
              return false;
            },
            switch: function(row, vm) {
              if(row.tenantId === vm.$store.state.session.tenantId){
                return true;
              }
              return false;
            }
        },
        columns: [
          {
              type: 'expand',
              width: 20,
              render: (h, params) => {
                  return h(ruleItems, {
                      props: {
                          splitRuleId: params.row.id,
                          splitRuleName: params.row.ruleName,
                          canModify: params.row.tenantId === this.$store.state.session.tenantId
                      }
                  })
              }
          },
          {
            title: this.$t('Menu.Pages.Logistics'),
            key: "logisticName"
          },
          {
            title: this.$t('Menu.Pages.LogisticChannels'),
            key: "logisticChannelName"
          },
          {
            title: this.$t('SplitRules.RuleName'),
            key: "ruleName"
          },
          {
            title: this.$t('SplitRules.MaxPackage'),
            width: 100,
            key: "maxPackage"
          },
          {
            title: this.$t('SplitRules.MaxWeight') + "(g)",
            width: 120,
            key: "maxWeight"
          },
          {
            title: this.$t('SplitRules.MaxTax') + "(AUD)",
            width: 120,
            key: "maxTax"
          },
          {
            title: this.$t('SplitRules.MaxPrice') + "(AUD)",
            width: 120,
            key: "maxPrice"
          },
          {
            title: this.$t('Public.IsActive'),
            width: 100,
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
        SplitRuleApi.Delete(id).then(()=>{
            callback();
        });
      });
      this.$on('on-createRow',(model,callback) =>{
        model.logisticChannelId = _this.cascaderValue[1];
        SplitRuleApi.Create(model).then(()=>{
            callback();
        });
      });
      this.$on('on-editRow',(model,callback) =>{
        model.logisticChannelId = _this.cascaderValue[1];
        SplitRuleApi.Update(model).then(()=>{
            callback();
        });
      });
      this.$on('on-switchRow',(id, isActive, callback) =>{
        SplitRuleApi.Switch(id, isActive).then(()=>{
            callback();
        });
      });
      this.$on('set-modalState',state => {
        _this.showSpecified = state === 'edit';
      });
  },
  async created(){
    var _this = this;
    LogisticChannelApi.GetOwn().then(req => {
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