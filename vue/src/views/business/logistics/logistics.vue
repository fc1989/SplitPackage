<template>
    <div>
        <simplePage ref="simplepage" :title="title" 
            :columnsetting="columnsetting" 
            :api="api"
            :newRule="newLogisticRule"
            :editRule="logisticRule"
            showSearchFilter
            :modalWidth="800">
            <template slot="search" slot-scope="slotProps">
                <Input v-model="slotProps.searchData.corporationName" :maxlength="50" :placeholder="$t('Logistics.CorporationName')" style="width:150px"></Input>
            </template>
            <template slot="appendBtn">
                <Button v-if="$store.state.session.tenantId !== null" @click="showModal" type="success">{{$t('Public.Import')}}</Button>
            </template>
            <template slot="newform" slot-scope="slotProps">
                <Tabs value="detail">
                    <TabPane :label="$t('Public.Details')" name="detail">
                        <FormItem :label="$t('Logistics.CorporationName')" prop="corporationName">
                            <Input v-model="slotProps.createModel.corporationName" :maxlength="200" :minlength="1"></Input>
                        </FormItem>
                        <FormItem :label="$t('Logistics.CorporationUrl')" prop="corporationUrl">
                            <Input v-model="slotProps.createModel.corporationUrl" :maxlength="50"></Input>
                        </FormItem>
                        <FormItem :label="$t('Logistics.LogisticCode')" prop="logisticCode">
                            <Input v-model="slotProps.createModel.logisticCode" :maxlength="50">
                                <span v-if="$store.state.session.tenantId" slot="prepend">{{$store.state.session.tenantId}}_</span>
                            </Input>
                        </FormItem>
                    </TabPane>
                </Tabs>
            </template>
            <template slot="editform" slot-scope="slotProps">
                <Tabs value="detail">
                    <TabPane :label="$t('Public.Details')" name="detail">
                        <FormItem :label="$t('Logistics.CorporationName')" prop="corporationName">
                            <Input v-model="slotProps.editModel.corporationName" :maxlength="200" :minlength="1"></Input>
                        </FormItem>
                        <FormItem :label="$t('Logistics.CorporationUrl')" prop="corporationUrl">
                            <Input v-model="slotProps.editModel.corporationUrl" :maxlength="50"></Input>
                        </FormItem>
                        <FormItem :label="$t('Logistics.LogisticCode')" prop="logisticCode">
                            <Input v-model="slotProps.editModel.logisticCode" :maxlength="50" disabled="disabled" ></Input>
                        </FormItem>
                        <FormItem>
                            <Checkbox v-model="slotProps.editModel.isActive">{{$t('Public.IsActive')}}</Checkbox>
                        </FormItem>
                    </TabPane>
                </Tabs>
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
import simplePage from "../../../components/simplepage.vue";
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
    }
  },
  data() {
    const validateFlag = (rule, value, callback) => {
      if (!value) {
        callback(new Error("logisticCode is required"));
      } else if(!/^[A-Za-z0-9]+$/.test(value))
      {
        callback(new Error("Number or combination of letters"));
      } else
      {
        LogisticApi.Verify(value).then(function(rep) {
          if (rep.data.result) {
            callback();
          } else {
            callback("logisticCode is exit");
          }
        });
      }
    };
    var _this = this;
    return {
      title: "Menu.Pages.Logistics",
      api: LogisticApi,
      newLogisticRule: {
        corporationName: [{ required: true }],
        logisticCode: [{ required: true, validator:validateFlag }]
      },
      logisticRule: {
        corporationName: [{ required: true }],
        logisticCode: [{ required: true }]
      },
      columnsetting: {
        actionOption: {
            edit: function(row,vm) {
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
      }
    };
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