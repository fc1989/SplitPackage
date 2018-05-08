<template>
    <simplePage ref="simplepage" :title="title" 
        :columnsetting="columnsetting" 
        :api="api"
        :newRule="newProductRule"
        :editRule="productRule"
        :createFormat="createFormat"
        showSearchFilter>
        <template slot="search" slot-scope="slotProps">
            <Input v-model="slotProps.searchData.productName" :maxlength="50" :placeholder="$t('Products.ProductName')" style="width:150px"></Input>
            <Input v-model="slotProps.searchData.sku" :maxlength="50" :placeholder="$t('Products.Sku')" style="width:150px"></Input>
            <Input v-model="slotProps.searchData.brand" :maxlength="50" :placeholder="$t('Products.Brand')" style="width:150px"></Input>
            <Input v-model="slotProps.searchData.ptid" :maxlength="50" :placeholder="$t('Menu.Pages.ProductClasses')" style="width:150px"></Input>
        </template>
        <template slot="newform" slot-scope="slotProps">
            <FormItem :label="$t('Products.ProductName')" prop="productName">
                <Input v-model="slotProps.createModel.productName" :maxlength="200" :minlength="1"></Input>
            </FormItem>
            <FormItem :label="$t('Products.Sku')" prop="sku">
                <Input v-model="slotProps.createModel.sku" :maxlength="50"></Input>
            </FormItem>
            <FormItem :label="$t('Menu.Pages.ProductClasses')" prop="ptid">
                <Input v-model="slotProps.createModel.ptid"></Input>
            </FormItem>
            <FormItem :label="$t('Products.Brand')" prop="brand">
                <Input v-model="slotProps.createModel.brand" :maxlength="50"></Input>
            </FormItem>
            <FormItem :label="$t('Products.Weight')" prop="weight">
                <Input-number v-model.number="slotProps.createModel.weight" style="width:100%"></Input-number>
            </FormItem>
            <FormItem :label="$t('Products.DeclarePrice')" prop="declarePrice">
                <Input-number v-model.number="slotProps.createModel.declarePrice" style="width:100%"></Input-number>
            </FormItem>
            <FormItem :label="$t('Products.DeclareTaxrate')" prop="declareTaxrate">
                <Input-number v-model.number="slotProps.createModel.declareTaxrate" style="width:100%"></Input-number>
            </FormItem>
        </template>
        <template slot="editform" slot-scope="slotProps">
            <FormItem :label="$t('Products.ProductName')" prop="productName">
                <Input v-model="slotProps.editModel.productName" :maxlength="200" :minlength="1"></Input>
            </FormItem>
            <FormItem :label="$t('Products.Sku')" prop="sku">
                <Input v-model="slotProps.editModel.sku" :maxlength="50"></Input>
            </FormItem>
            <FormItem :label="$t('Menu.Pages.ProductClasses')" prop="ptid">
                <Input v-model="slotProps.editModel.ptid"></Input>
            </FormItem>
            <FormItem :label="$t('Products.Brand')" prop="brand">
                <Input v-model="slotProps.editModel.brand" :maxlength="50"></Input>
            </FormItem>
            <FormItem :label="$t('Products.Weight')" prop="weight">
                <Input-number v-model.number="slotProps.editModel.weight" style="width:100%"></Input-number>
            </FormItem>
            <FormItem :label="$t('Products.DeclarePrice')" prop="declarePrice">
                <Input-number v-model.number="slotProps.editModel.declarePrice" style="width:100%"></Input-number>
            </FormItem>
            <FormItem :label="$t('Products.DeclareTaxrate')" prop="declareTaxrate">
                <Input-number v-model.number="slotProps.editModel.declareTaxrate" style="width:100%"></Input-number>
            </FormItem>
            <FormItem>
                <Checkbox v-model="slotProps.editModel.isActive">{{$t('Public.IsActive')}}</Checkbox>
            </FormItem>
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
  data() {
    var _this = this;
    const cf = function() {
      _this.options = [];
      return {
        productName: null,
        sku: null,
        brand: null,
        weight: 0,
        declarePrice: 0,
        declareTaxrate: 0,
        ptid: null
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
      title: "Menu.Pages.Products",
      createFormat: cf,
      api: ProductApi,
      newProductRule: {
        productName: [{ required: true }],
        sku: [{ required: true, validator: validateSku }],
        weight: [{ type: "number" }],
        ptid: [{required: true}],
        declarePrice: [{ type: "number" }],
        declareTaxrate: [{ type: "number" }]
      },
      productRule: {
        productName: [{ required: true }],
        productNo: [{ required: true }],
        weight: [{ type: "number" }],
        ptid: [{required: true}],
        declarePrice: [{ type: "number" }],
        declareTaxrate: [{ type: "number" }]
      },
      columnsetting: {
        needAction: false,
        columns: [
          {
            title: this.$t("Products.ProductName"),
            key: "productName"
          },
          {
            title: this.$t("Products.Sku"),
            key: "sku"
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
            title: this.$t("Products.DeclarePrice"),
            key: "declarePrice"
          },
          {
            title: this.$t("Products.DeclareTaxrate"),
            key: "declareTaxrate"
          },
          {
            title: this.$t("Menu.Pages.ProductClasses"),
            key: "ptid"
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
                      click: async () => {
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