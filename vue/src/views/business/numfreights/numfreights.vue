<template>
    <simplePage ref="simplepage" :title="title" 
        :columnsetting="columnsetting" 
        :api="api"
        :newRule="newNumFreightRule"
        :editRule="numFreightRule"
        :createFormat="createFormat">
        <template slot="newform" slot-scope="slotProps">
            <FormItem :label="$t('NumFreights.PackagePrice')" prop="packagePrice">
                <Input-number v-model.number="slotProps.createModel.packagePrice" style="width:100%"></Input-number>
            </FormItem>
            <FormItem :label="$t('NumFreights.ProductNum')" prop="productNum">
                <Input-number v-model.number="slotProps.createModel.productNum" style="width:100%"></Input-number>
            </FormItem>
            <FormItem :label="$t('Menu.Pages.LogisticLines')" prop="logisticLineId">
                <Select
                    v-model="slotProps.createModel.logisticLineId"
                    filterable
                    remote
                    :remote-method="remoteLLMethod"
                    :loading="loading2">
                    <Option v-for="(option) in options" :value="option.value" :key="option.value">{{option.label}}</Option>
                </Select>
            </FormItem>
        </template>
        <template slot="editform" slot-scope="slotProps">
            <FormItem :label="$t('NumFreights.PackagePrice')" prop="packagePrice">
                <Input-number v-model.number="slotProps.editModel.packagePrice" style="width:100%"></Input-number>
            </FormItem>
            <FormItem :label="$t('NumFreights.ProductNum')" prop="productNum">
                <Input-number v-model.number="slotProps.editModel.productNum" style="width:100%"></Input-number>
            </FormItem>
            <FormItem :label="$t('Menu.Pages.LogisticLines')" prop="logisticLineId">
                <Select
                    v-model="slotProps.editModel.logisticLineId"
                    :label="label"
                    disabled
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
import NumFreightApi from "@/api/numFreight";
import LogisticLineApi from "@/api/logisticline";

export default {
  components: {
    simplePage
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
        packagePrice: 0,
        productNum: 0
      };
    };
    return {
      title: "Menu.Pages.NumFreights",
      loading2: false,
      options: [],
      label:null,
      createFormat: cf,
      api: NumFreightApi,
      newNumFreightRule: {
        packagePrice: [{ type: "number" }],
        productNum: [{ type: "number" }],
        logisticLineId: [{ required: true }]
      },
      numFreightRule: {
        packagePrice: [{ type: "number" }],
        productNum: [{ type: "number" }],
        logisticLineId: [{ required: true }]
      },
      columnsetting: {
        needAction: false,
        columns: [
          {
            title: this.$t('NumFreights.PackagePrice'),
            key: "packagePrice"
          },
          {
            title: this.$t('NumFreights.ProductNum'),
            key: "productNum"
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