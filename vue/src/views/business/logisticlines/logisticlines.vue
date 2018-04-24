<template>
    <simplePage ref="simplepage" :title="title" 
        :columnsetting="columnsetting" 
        :api="api"
        :newRule="newLogisticLineRule"
        :editRule="logisticLineRule">
        <template slot="newform" slot-scope="slotProps">
            <FormItem :label="$t('LogisticLines.LineName')" prop="lineName">
                <Input v-model="slotProps.createModel.lineName" :maxlength="200" :minlength="1"></Input>
            </FormItem>
            <FormItem :label="$t('LogisticLines.LineCode')" prop="lineCode">
                <Input v-model="slotProps.createModel.lineCode" :maxlength="50"></Input>
            </FormItem>
            <FormItem :label="$t('Menu.Pages.Logistics')" prop="logisticId">
                <Select
                    v-model="slotProps.createModel.logisticId"
                    clearable
                    filterable
                    remote
                    :remote-method="remoteLMethod"
                    :loading="loading2">
                    <Option v-for="(option) in options" :value="option.value" :key="option.value">{{option.label}}</Option>
                </Select>
            </FormItem>
        </template>
        <template slot="editform" slot-scope="slotProps">
            <FormItem :label="$t('LogisticLines.LineName')" prop="lineName">
                <Input v-model="slotProps.editModel.lineName" :maxlength="200" :minlength="1"></Input>
            </FormItem>
            <FormItem :label="$t('LogisticLines.LineCode')" prop="lineCode">
                <Input v-model="slotProps.editModel.lineCode" :maxlength="50" disabled="disabled" ></Input>
            </FormItem>
            <FormItem :label="$t('Menu.Pages.Logistics')" prop="logisticId">
                <Select
                    v-model="slotProps.editModel.logisticId"
                    :label="label"
                    disabled
                    filterable
                    remote
                    :remote-method="remoteLMethod"
                    :loading="loading2">
                    <Option v-for="(option) in options" :value="option.value" :key="option.value">{{option.label}}</Option>
                </Select>
            </FormItem>
            <FormItem>
                <Checkbox v-model="slotProps.editModel.isActive">{{$t('Public.IsActive')}}</Checkbox>
            </FormItem>
        </template>
    </simplePage>
</template>
<script>
import simplePage from "../../../components/simplepage.vue";
import LogisticLineApi from "@/api/logisticline";
import LogisticApi from "@/api/logistic";

export default {
  components: {
    simplePage
  },
  methods:{
    remoteLMethod(query) {
      if (query !== "") {
        let _this = this;
        this.loading2 = true;
        LogisticApi.Query(query, null).then(function(req){
          _this.options = req.data.result;
          _this.loading2 = false;
        });
      } else {
        this.options = null;
      }
    }
  },
  data() {
    var _this = this;
    return {
      title: "Menu.Pages.LogisticLines",
      label: null,
      loading2: false,
      options: [],
      api: LogisticLineApi,
      newLogisticLineRule: {
        lineName: [{ required: true }],
        lineCode: [{ required: true }],
        logisticId: [{ required: true }]
      },
      logisticLineRule: {
        lineName: [{ required: true }],
        lineCode: [{ required: true }],
        logisticId: [{ required: true }]
      },
      columnsetting: {
        needAction: false,
        columns: [
          {
            title: this.$t('LogisticLines.LineName'),
            key: "lineName"
          },
          {
            title: this.$t('LogisticLines.LineCode'),
            key: "lineCode"
          },
          {
            title: this.$t('Menu.Pages.Logistics'),
            key: "logisticName"
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
          },
          {
            title: this.$t("Public.Actions"),
            key: "action",
            width: 150,
            render: (h, params) => {
              return h("div", [
                h(
                  "Button",
                  {
                    props: {
                      type: "primary",
                      size: "small"
                    },
                    style: {
                      marginRight: "5px"
                    },
                    on: {
                      click: async () => {
                        let req = await LogisticApi.Query(
                          "",
                          [params.row.logisticId]
                        );
                        if(req.data.result.length){
                          _this.options = req.data.result;
                          _this.label = req.data.result[0].label;
                        }
                        else{
                            _this.options = [];
                            _this.label = "";
                        }
                        _this.$refs.simplepage.editModel = params.row;
                        _this.$refs.simplepage.showEditModal = true;
                      }
                    }
                  },
                  this.$t("Public.Edit")
                ),
                h(
                  "Button",
                  {
                    props: {
                      type: "error",
                      size: "small"
                    },
                    on: {
                      click: async () => {
                        this.$Modal.confirm({
                          title: this.$t(""),
                          content:
                            this.$t("Public.Delete") + this.$t(this.title),
                          okText: this.$t("Public.Yes"),
                          cancelText: this.$t("Public.No"),
                          onOk: async () => {
                            await _this.api.Delete(params.row.id);
                          }
                        });
                      }
                    }
                  },
                  this.$t("Public.Delete")
                )
              ]);
            }
          }
        ]
      }
    };
  }
};
</script>