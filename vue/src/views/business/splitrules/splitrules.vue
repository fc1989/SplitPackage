<template>
    <simplePage ref="simplepage" :title="title" 
        :columnsetting="columnsetting" 
        :api="api"
        :newRule="newSplitRuleRule"
        :editRule="splitRuleRule"
        :createFormat="createFormat">
        <template slot="newform" slot-scope="slotProps">
            <Tabs value="detail">
                <TabPane :label="$t('Public.Details')" name="detail">
                  <FormItem :label="$t('SplitRules.MaxPackage')" prop="maxPackage">
                      <Input-number v-model.number="slotProps.createModel.maxPackage" style="width:100%"></Input-number>
                  </FormItem>
                  <FormItem :label="$t('SplitRules.MaxWeight')" prop="maxWeight">
                      <Input-number v-model.number="slotProps.createModel.maxWeight" style="width:100%"></Input-number>
                  </FormItem>
                  <FormItem :label="$t('SplitRules.MaxTax')" prop="maxTax">
                      <Input-number v-model.number="slotProps.createModel.maxTax" style="width:100%"></Input-number>
                  </FormItem>
                  <FormItem :label="$t('SplitRules.MaxPrice')" prop="maxPrice">
                      <Input-number v-model.number="slotProps.createModel.maxPrice" style="width:100%"></Input-number>
                  </FormItem>
                  <FormItem :label="$t('Menu.Pages.LogisticLines')" prop="logisticLineId">
                      <Select
                          v-model="slotProps.createModel.logisticLineId"
                          clearable
                          filterable
                          remote
                          :remote-method="remoteLLMethod"
                          :loading="loading2">
                          <Option v-for="(option) in options" :value="option.value" :key="option.value">{{option.label}}</Option>
                      </Select>
                  </FormItem>
                </TabPane>
                <TabPane :label="$t('Menu.Pages.ProductClasses')" name="productClass" :disabled="false" style="overflow: auto;max-height: 400px;">
                  <rule-items v-model="slotProps.createModel.ruleItems"></rule-items>
                </TabPane>
            </Tabs>
        </template>
        <template slot="editform" slot-scope="slotProps">
            <FormItem :label="$t('SplitRules.MaxPackage')" prop="maxPackage">
                <Input-number v-model.number="slotProps.editModel.maxPackage" style="width:100%"></Input-number>
            </FormItem>
            <FormItem :label="$t('SplitRules.MaxWeight')" prop="maxWeight">
                <Input-number v-model.number="slotProps.editModel.maxWeight" style="width:100%"></Input-number>
            </FormItem>
            <FormItem :label="$t('SplitRules.MaxTax')" prop="maxTax">
                <Input-number v-model.number="slotProps.editModel.maxTax" style="width:100%"></Input-number>
            </FormItem>
            <FormItem :label="$t('SplitRules.MaxPrice')" prop="maxPrice">
                <Input-number v-model.number="slotProps.editModel.maxPrice" style="width:100%"></Input-number>
            </FormItem>
            <FormItem :label="$t('Menu.Pages.LogisticLines')" prop="logisticLineId">
                <Select
                    v-model="slotProps.editModel.logisticLineId"
                    disabled
                    :label="label"
                    filterable
                    remote
                    :remote-method="remoteLLMethod"
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
import ruleItems from './ruleitems'
import SplitRuleApi from "@/api/splitRule";
import LogisticLineApi from "@/api/logisticline";

export default {
  components: {
    simplePage,
    ruleItems
  },
  methods:{
    remoteLLMethod(query) {
      if (query !== "") {
        let _this = this;
        this.loading2 = true;
        LogisticLineApi.Query(query, null).then(function(req){
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
    const cf = function() {
      _this.options = [];
      return {
        maxPackage: 0,
        maxWeight: 0,
        maxTax: 0,
        maxPrice: 0,
        ruleItems: []
      };
    };
    return {
      title: "Menu.Pages.SplitRules",
      label: null,
      loading2: false,
      options: [],
      createFormat: cf,
      api: SplitRuleApi,
      newSplitRuleRule: {
        logisticLineId: [{ required: true }],
        maxPackage: [{ required: true }],
        maxWeight: [{ required: true }],
        maxTax: [{ required: true }],
        maxPrice: [{ required: true }]
      },
      splitRuleRule: {
        logisticLineId: [{ required: true }],
        maxPackage: [{ required: true }],
        maxWeight: [{ required: true }],
        maxTax: [{ required: true }],
        maxPrice: [{ required: true }]
      },
      columnsetting: {
        needAction: false,
        columns: [
          {
            title: this.$t('SplitRules.MaxPackage'),
            key: "maxPackage"
          },
          {
            title: this.$t('SplitRules.MaxWeight'),
            key: "maxWeight"
          },
          {
            title: this.$t('SplitRules.MaxTax'),
            key: "maxTax"
          },
          {
            title: this.$t('SplitRules.MaxPrice'),
            key: "maxPrice"
          },
          {
            title: this.$t('Menu.Pages.LogisticLines'),
            key: "logisticLineName"
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
                        let req = await LogisticLineApi.Query(
                          "",
                          [params.row.logisticLineId]
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