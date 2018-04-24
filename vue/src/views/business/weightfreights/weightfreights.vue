<template>
    <simplePage ref="simplepage" :title="title" 
        :columnsetting="columnsetting" 
        :api="api"
        :newRule="newWeightFreightRule"
        :editRule="weightFreightRule"
        :createFormat="createFormat">
        <template slot="newform" slot-scope="slotProps">
            <FormItem :label="$t('WeightFreights.StartingWeight')" prop="startingWeight">
                <Input-number v-model.number="slotProps.createModel.startingWeight" style="width:100%"></Input-number>
            </FormItem>
            <FormItem :label="$t('WeightFreights.StartingPrice')" prop="startingPrice">
                <Input-number v-model.number="slotProps.createModel.startingPrice" style="width:100%"></Input-number>
            </FormItem>
            <FormItem :label="$t('WeightFreights.StepWeight')" prop="stepWeight">
                <Input-number v-model.number="slotProps.createModel.stepWeight" style="width:100%"></Input-number>
            </FormItem>
            <FormItem :label="$t('WeightFreights.Price')" prop="price">
                <Input-number v-model.number="slotProps.createModel.price" style="width:100%"></Input-number>
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
            <FormItem :label="$t('WeightFreights.StartingWeight')" prop="startingWeight">
                <Input-number v-model.number="slotProps.editModel.startingWeight" style="width:100%"></Input-number>
            </FormItem>
            <FormItem :label="$t('WeightFreights.StartingPrice')" prop="startingPrice">
                <Input-number v-model.number="slotProps.editModel.startingPrice" style="width:100%"></Input-number>
            </FormItem>
            <FormItem :label="$t('WeightFreights.StepWeight')" prop="stepWeight">
                <Input-number v-model.number="slotProps.editModel.stepWeight" style="width:100%"></Input-number>
            </FormItem>
            <FormItem :label="$t('WeightFreights.Price')" prop="price">
                <Input-number v-model.number="slotProps.editModel.price" style="width:100%"></Input-number>
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
import WeightFreightApi from "@/api/weightFreight";
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
        startingWeight: 0,
        startingPrice: 0,
        stepWeight: 0,
        price: 0
      };
    };
    return {
      title: "Menu.Pages.WeightFreights",
      loading2: false,
      options: [],
      label:null,
      api: WeightFreightApi,
      createFormat: cf,
      newWeightFreightRule: {
        startingWeight: [{ type: "number" }],
        startingPrice: [{ type: "number" }],
        stepWeight: [{ type: "number" }],
        price: [{ type: "number" }],
        logisticLineId: [{ required:true }]
      },
      weightFreightRule: {
        startingWeight: [{ type: "number" }],
        startingPrice: [{ type: "number" }],
        stepWeight: [{ type: "number" }],
        price: [{ type: "number" }],
        logisticLineId: [{ required:true }]
      },
      columnsetting: {
        needAction: false,
        columns: [
          {
            title: this.$t('WeightFreights.StartingWeight'),
            key: "startingWeight"
          },
          {
            title: this.$t('WeightFreights.StartingPrice'),
            key: "startingPrice"
          },
          {
            title: this.$t('WeightFreights.StepWeight'),
            key: "StepWeight"
          },
          {
            title: this.$t('WeightFreights.Price'),
            key: "price"
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