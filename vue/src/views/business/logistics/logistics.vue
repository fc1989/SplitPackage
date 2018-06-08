<template>
    <div>
        <simplePage ref="simplepage"
            :title="title" 
            :columnsetting="columnsetting" 
            :rule="rule"
            showSearchFilter
            :searchPage="getPage"
            :getCreateModel="getCreateModel"
            :getEditModel="getEditModel">
            <template slot="search" slot-scope="slotProps">
            <Input v-model="slotProps.searchData.corporationName" :maxlength="50" :placeholder="$t('Logistics.CorporationName')" style="width:150px"></Input>
            </template>
            <template slot="appendSearchBtn">
                <Button v-if="$store.state.session.tenantId !== null" @click="showModal" type="success">{{$t('Public.Import')}}</Button>
            </template>
            <template slot="modalForm" slot-scope="slotProps">
                <FormItem :label="$t('Logistics.CorporationName')" prop="corporationName">
                    <Input v-model="slotProps.model.corporationName" :maxlength="200" :minlength="1"></Input>
                </FormItem>
                <FormItem :label="$t('Logistics.CorporationUrl')" prop="corporationUrl">
                    <Input v-model="slotProps.model.corporationUrl" :maxlength="50"></Input>
                </FormItem>
                <FormItem v-if="showSpecified" :label="$t('Logistics.LogisticCode')" prop="logisticCode" :key="1">
                    <Input v-model="slotProps.model.logisticCode" disabled="disabled" ></Input>
                </FormItem>
                <FormItem v-else :label="$t('Logistics.LogisticCode')" prop="logisticCode" :key="2">
                    <Input v-model="slotProps.model.logisticCode" :maxlength="50" :disabled="showSpecified">
                        <span v-if="$store.state.session.tenantId" slot="prepend">{{$store.state.session.tenantId}}_</span>
                    </Input>
                </FormItem>
                <FormItem>
                    <Checkbox v-model="slotProps.model.isActive" disabled>{{$t('Public.IsActive')}}</Checkbox>
                </FormItem>
            </template>
        </simplePage>
        <Modal v-model="importState.showImportModal" :title="$t('Public.Import') + $t('Menu.Pages.LogisticChannels')">
            <div>
                <Transfer
                    :data="importState.systemLogisticChannel"
                    :target-keys="importState.importLogisticChannel"
                    :render-format="importRender"
                    @on-change="importHandleChange"
                    filterable
                    :filter-method="filterLogisticChannel"
                    :filterPlaceholder="$t('Logistics.FilterPoint')">
                </Transfer>
            </div>
            <div slot="footer">
                <Button @click="importState.showImportModal=false">{{$t('Public.Cancel')}}</Button>
                <Button @click="importLogisticChannels" type="primary">{{$t('Public.Save')}}</Button>
            </div>
        </Modal>
    </div>
</template>

<style>
    .ivu-table .demo-table-info-row td{
        background-color: #f2f5ee;
    }
</style>

<script>
import simplePage from "../../../components/simplepagev1.vue";
import LogisticChannels from "./logisticchannels";
import LogisticApi from "@/api/logistic";
import LogisticChannelApi from "@/api/logisticchannel";

export default {
  components: {
    simplePage,
    LogisticChannels
  },
  methods:{
    showModal(){
        var _this = this;
        LogisticChannelApi.GetImportState().then(req => {
            _this.importState.showImportModal = true;
            _this.importState.systemLogisticChannel = req.data.result.systemLogisticChannel.map((vl, index, arr) => { return {
                key: vl.id,
                label: vl.channelName,
                description: vl.logisticName,
                disabled: false
            };});
            _this.importState.importLogisticChannel = req.data.result.importLogisticChannel;
        });
    },
    filterLogisticChannel(data, query){
        var isMatch = data.description.indexOf(query) > -1;
        if(!isMatch){
            data.disabled = true;
        }
        else{
            data.disabled = false;
        }
        return isMatch;
    },
    importRender(item){
        return item.label + "(" + item.description + ")";
    },
    importHandleChange(newTargetKeys, direction, moveKeys){
        this.importState.importLogisticChannel = newTargetKeys;
    },
    importLogisticChannels(){
        LogisticChannelApi.CustomerImport(this.importState.importLogisticChannel).then(req => {
            this.importState.showImportModal = false;
            this.$refs.simplepage.getpage();
        });
    },
    async getPage(filter){
        var rep = await LogisticApi.Search(filter);
        return rep.data.result;
    },
    getCreateModel(){
      return {
        isActive: true
      };
    },
    async getEditModel(row){
      return row;
    }
  },
  data() {
    var _this = this;
    const validateFlag = (rule, value, callback) => {
      if (!value) {
        callback(new Error("logisticCode is required"));
      } else if(!/^[A-Za-z0-9]+[\s,_]*[A-Za-z0-9]*$/.test(value))
      {
        callback(new Error("Number or combination of letters"));
      } else
      {
        if(_this.showSpecified){
            callback();
            return
        }
        LogisticApi.Verify(value).then(function(rep) {
          if (rep.data.result) {
            callback();
          } else {
            callback("logisticCode is exit");
          }
        });
      }
    };
    return {
      title: "Menu.Pages.Logistics",
      rule: {
        corporationName: [{ required: true, trigger: 'ignore' }],
        logisticCode: [{ required: true, validator:validateFlag, trigger: 'ignore' }]
      },
      columnsetting: {
        actionOption: {
            edit: function(row,vm) {
                if(row.tenantId === vm.$store.state.session.tenantId){
                    return true;
                }
                return false;
            },
            switch: function(row,vm){
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
                  return h(LogisticChannels, {
                      props: {
                          logisticId: params.row.id,
                          logisticName: params.row.corporationName,
                          isImport: params.row.tenantId !== _this.$store.state.session.tenantId
                      }
                  })
              }
          },
          {
              title: this.$t('Logistics.CorporationName'),
              key: "corporationName"
          },
          {
            title: this.$t('Logistics.CorporationUrl'),
            key: "corporationUrl"
          },
          {
            title: this.$t('Logistics.LogisticCode'),
            key: "logisticCode"
          },
          {
              title: this.$t('Logistics.Source'),
              render: (h, params) => {
                    var source = null;
                    if(params.row.tenantId === _this.$store.state.session.tenantId){
                        source = _this.$t('Logistics.Myself');
                    } else {
                        source = _this.$t('Public.Import');
                    }
                    return h("span",source);
              }
          },
          {
            title: this.$t('Public.IsActive'),
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
      importState:{
        showImportModal: false,
        systemLogisticChannel: [],
        importLogisticChannel: []
      },
      showSpecified: null,
    };
  },
  mounted(){
      var _this = this;
      this.$on('on-createRow',(model,callback) =>{
        LogisticApi.Create(model).then(()=>{
            callback();
        });
      });
      this.$on('on-editRow',(model,callback) =>{
        LogisticApi.Update(model).then(()=>{
            callback();
        });
      });
      this.$on('on-switchRow',(id, isActive, callback) =>{
        LogisticApi.Switch(id, isActive).then(()=>{
            callback();
        });
      });
      this.$on('set-modalState',state => {
        _this.showSpecified = state === 'edit';
      });
  },
  created:function(){
      if(!this.$store.state.app.enumInformation.channelType){
          var _this = this;
          LogisticChannelApi.GetOptions().then(req=>{
            _this.$store.commit('setLogisticChannel', req.data.result);
          });
      }
  }
};
</script>