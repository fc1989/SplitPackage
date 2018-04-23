<template>
    <simplePage ref="simplepage" :title="title" 
        :columnsetting="columnsetting" 
        :api="api"
        :newRule="newProductRule"
        :editRule="productRule"
        :createFormat="createFormat">
        <template slot="newform" slot-scope="slotProps">
            <Tabs>
                <TabPane :label="$t('Public.Details')">
                    <FormItem :label="$t('Products.ProductName')" prop="productName">
                        <Input v-model="slotProps.createModel.productName" :maxlength="200" :minlength="1"></Input>
                    </FormItem>
                    <FormItem :label="$t('Products.AbbreName')" prop="abbreName">
                        <Input v-model="slotProps.createModel.abbreName" :maxlength="50"></Input>
                    </FormItem>
                    <FormItem :label="$t('Products.ProductNo')" prop="productNo">
                        <Input v-model="slotProps.createModel.productNo" :maxlength="50"></Input>
                    </FormItem>
                    <FormItem :label="$t('Products.Sku')" prop="sku">
                        <Input v-model="slotProps.createModel.sku" :maxlength="50"></Input>
                    </FormItem>
                    <FormItem :label="$t('Products.TaxNo')" prop="taxNo">
                        <Input v-model="slotProps.createModel.taxNo" :maxlength="20"></Input>
                    </FormItem>
                    <FormItem :label="$t('Products.Brand')" prop="brand">
                        <Input v-model="slotProps.createModel.brand" :maxlength="50"></Input>
                    </FormItem>
                    <FormItem :label="$t('Products.Weight')" prop="weight">
                        <Input-number v-model.number="slotProps.createModel.weight" style="width:100%"></Input-number>
                    </FormItem>
                </TabPane>
                <TabPane :label="$t('Menu.Pages.ProductClasses')">
                    <FormItem :label="$t('Menu.Pages.ProductClasses')">
                        <Select
                            v-model="slotProps.createModel.productClassIds"
                            multiple
                            filterable
                            remote
                            :remote-method="remotePCMethod"
                            :loading="loading2">
                            <Option v-for="(option, index) in options" :value="option.value" :key="index">{{option.label}}</Option>
                        </Select>
                    </FormItem>
                </TabPane>
            </Tabs>
        </template>
        <template slot="editform" slot-scope="slotProps">
            <Tabs value="detail">
                <TabPane :label="$t('Public.Details')" name="detail">
                    <FormItem :label="$t('Products.ProductName')" prop="productName">
                        <Input v-model="slotProps.editModel.productName" :maxlength="200" :minlength="1"></Input>
                    </FormItem>
                    <FormItem :label="$t('Products.AbbreName')" prop="abbreName">
                        <Input v-model="slotProps.editModel.abbreName" :maxlength="50"></Input>
                    </FormItem>
                    <FormItem :label="$t('Products.ProductNo')" prop="productNo">
                        <Input v-model="slotProps.editModel.productNo" :maxlength="50"></Input>
                    </FormItem>
                    <FormItem :label="$t('Products.Sku')" prop="sku">
                        <Input v-model="slotProps.editModel.sku" :maxlength="50" ></Input>
                    </FormItem>
                    <FormItem :label="$t('Products.TaxNo')" prop="taxNo">
                        <Input v-model="slotProps.editModel.taxNo" :maxlength="20"></Input>
                    </FormItem>
                    <FormItem :label="$t('Products.Brand')" prop="brand">
                        <Input v-model="slotProps.editModel.brand" :maxlength="50"></Input>
                    </FormItem>
                    <FormItem :label="$t('Products.Weight')" prop="weight">
                        <Input-number v-model.number="slotProps.editModel.weight" style="width:100%"></Input-number>
                    </FormItem>
                    <FormItem>
                        <Checkbox v-model="slotProps.editModel.isActive">{{$t('Public.IsActive')}}</Checkbox>
                    </FormItem>
                </TabPane>
                <TabPane :label="$t('Menu.Pages.ProductClasses')">
                    <FormItem :label="$t('Menu.Pages.ProductClasses')">
                        <Select ref="es"
                            :label="label"
                            v-model="slotProps.editModel.productClassIds"
                            filterable
                            remote
                            :remote-method="remotePCMethod"
                            :loading="loading2">
                            <Option v-for="(option, index) in options" :value="option.value" :key="index">{{option.label}}</Option>
                        </Select>
                    </FormItem>
                </TabPane>
            </Tabs>
        </template>
    </simplePage>
</template>
<script>
import simplePage from "../../../components/simplepage.vue";
import ProductClassApi from "@/api/productclass";
import ProductApi from "@/api/product";

export default {
  components: {
    simplePage
  },
  methods: {
    remotePCMethod(query) {
      if (query !== "") {
        let _this = this;
        this.loading2 = true;
        ProductClassApi.Query(query, null).then(function(req) {
          _this.loading2 = false;
          _this.options = req.data.result;
        });
      } else {
        this.options = [];
      }
    }
  },
  data() {
    var _this = this;
    const cf = function() {
      _this.options = [];
      return {
        productName: null,
        abbreName: null,
        productNo: null,
        sku: null,
        taxNo: null,
        brand: null,
        weight: 0,
        productClassIds: []
      };
    };
    const validateSku = (rule, value, callback) => {
      if (!value) {
        callback(new Error("sku is required"));
      } else {
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
      label:null,
      title: "Menu.Pages.Products",
      loading2: false,
      options: [],
      createFormat: cf,
      api: ProductApi,
      newProductRule: {
        productName: [{ required: true }],
        productNo: [{ required: true }],
        sku: [{ required: true, validator: validateSku }],
        weight: [{ type: "number" }]
      },
      productRule: {
        productName: [{ required: true, trigger: "blur" }],
        productNo: [{ required: true, trigger: "blur" }],
        weight: [{ type: "number" }]
      },
      columnsetting: {
        needAction: false,
        columns: [
          {
            title: this.$t("Products.ProductName"),
            key: "productName"
          },
          {
            title: this.$t("Products.AbbreName"),
            key: "abbreName"
          },
          {
            title: this.$t("Products.ProductNo"),
            key: "productNo"
          },
          {
            title: this.$t("Products.Sku"),
            key: "sku"
          },
          {
            title: this.$t("Products.TaxNo"),
            key: "taxNo"
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
            title: this.$t("Public.IsActive"),
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
                      click: () => {
                        _this.$refs.simplepage.editModel = params.row;
                        _this.$refs.simplepage.showEditModal = true;
                        ProductClassApi.Query(
                          "",
                          params.row.productClassIds
                        ).then(function(req) {
                          _this.options = req.data.result;
                          _this.label = _this.options[0].label;
                        });
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