<template>
    <simplePage ref="simplepage" :title="title" 
        :columnsetting="columnsetting" 
        :api="api"
        :newRule="newProductClassRule"
        :editRule="productClassRule"
        :createFormat="createFormat">
        <template slot="newform" slot-scope="slotProps">
            <FormItem :label="$t('ProductClasses.ClassName')" prop="className">
                <Input v-model="slotProps.createModel.className" :maxlength="200" :minlength="1"></Input>
            </FormItem>
            <FormItem :label="$t('ProductClasses.PTId')" prop="ptid">
                <Input v-model="slotProps.createModel.ptid" :maxlength="50"></Input>
            </FormItem>
            <FormItem :label="$t('ProductClasses.PostTaxRate')" prop="postTaxRate">
                <InputNumber v-model="slotProps.createModel.postTaxRate" style="width:100%"></InputNumber>
            </FormItem>
            <FormItem :label="$t('ProductClasses.BCTaxRate')" prop="bcTaxRate">
                <InputNumber v-model="slotProps.createModel.bcTaxRate" style="width:100%"></InputNumber>
            </FormItem>
        </template>
        <template slot="editform" slot-scope="slotProps">
            <FormItem :label="$t('ProductClasses.ClassName')" prop="className">
                <Input v-model="slotProps.editModel.className" :maxlength="200" :minlength="1"></Input>
            </FormItem>
            <FormItem :label="$t('ProductClasses.PTId')" prop="ptid">
                <Input v-model="slotProps.editModel.ptid" :maxlength="50" disabled="disabled"></Input>
            </FormItem>
            <FormItem :label="$t('ProductClasses.PostTaxRate')" prop="postTaxRate">
                <InputNumber v-model="slotProps.editModel.postTaxRate" style="width:100%"></InputNumber>
            </FormItem>
            <FormItem :label="$t('ProductClasses.BCTaxRate')" prop="bcTaxRate">
                <InputNumber v-model="slotProps.editModel.bcTaxRate" style="width:100%"></InputNumber>
            </FormItem>
            <FormItem>
                <Checkbox v-model="slotProps.editModel.isActive">{{$t('Public.IsActive')}}</Checkbox>
            </FormItem>
        </template>
    </simplePage>
</template>
<script>
import simplePage from "../../../components/simplepage.vue";
import ProductClassApi from "@/api/productClass";

export default {
  components: {
    simplePage
  },
  data() {
    const validatePTId = (rule, value, callback) => {
      if (!value) {
        callback(new Error("PTId is required"));
      } else {
        ProductClassApi.Verify(value).then(function(rep) {
          if (rep.data.result) {
            callback();
          } else {
            callback("PTId is exit");
          }
        });
      }
    };
    const cf = function() {
      return {
        postTaxRate: 0,
        bcTaxRate: 0
      };
    };
    return {
      title: "Menu.Pages.ProductClasses",
      api: ProductClassApi,
      createFormat: cf,
      newProductClassRule: {
        className: [{ required: true }],
        ptid: [{ required: true,validator:validatePTId }],
        postTaxRate: [{ type: "number" }],
        bcTaxRate: [{ type: "number" }]
      },
      productClassRule: {
        className: [{ required: true }],
        ptid: [{ required: true }],
        postTaxRate: [{ type: "number" }],
        bcTaxRate: [{ type: "number" }]
      },
      columnsetting: {
        needAction: true,
        columns: [
          {
            title: this.$t("ProductClasses.ClassName"),
            key: "className"
          },
          {
            title: this.$t("ProductClasses.PTId"),
            key: "ptid"
          },
          {
            title: this.$t("ProductClasses.PostTaxRate"),
            key: "postTaxRate"
          },
          {
            title: this.$t("ProductClasses.BCTaxRate"),
            key: "bcTaxRate"
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
          }
        ]
      }
    };
  }
};
</script>