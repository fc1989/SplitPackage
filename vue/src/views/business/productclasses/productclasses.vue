<template>
    <simplePage ref="simplepage" :title="title" 
        :columnsetting="columnsetting" 
        :api="api"
        :newRule="newProductClassRule"
        :editRule="productClassRule">
        <template slot="newform" slot-scope="slotProps">
            <Tabs value="detail">
                <TabPane :label="$t('Public.Details')" name="detail">
                    <FormItem :label="$t('ProductClasses.ClassName')" prop="className">
                        <Input v-model="slotProps.createModel.ClassName" :maxlength="200" :minlength="1"></Input>
                    </FormItem>
                    <FormItem :label="$t('ProductClasses.PTId')" prop="pTId">
                        <Input v-model="slotProps.createModel.pTId" :maxlength="50"></Input>
                    </FormItem>
                    <FormItem :label="$t('ProductClasses.PostTaxRate')" prop="postTaxRate">
                        <Input-number v-model.number="slotProps.createModel.postTaxRate" style="width:100%"></Input-number>
                    </FormItem>
                    <FormItem :label="$t('ProductClasses.BCTaxRate')" prop="bCTaxRate">
                        <Input-number v-model.number="slotProps.createModel.bCTaxRate" style="width:100%"></Input-number>
                    </FormItem>
                </TabPane>
            </Tabs>
        </template>
        <template slot="editform" slot-scope="slotProps">
            <Tabs value="detail">
                <TabPane :label="$t('Public.Details')" name="detail">
                    <FormItem :label="$t('ProductClasses.ClassName')" prop="className">
                        <Input v-model="slotProps.editModel.ClassName" :maxlength="200" :minlength="1"></Input>
                    </FormItem>
                    <FormItem :label="$t('ProductClasses.PTId')" prop="pTId">
                        <Input v-model="slotProps.editModel.pTId" :maxlength="50"></Input>
                    </FormItem>
                    <FormItem :label="$t('ProductClasses.PostTaxRate')" prop="postTaxRate">
                        <Input-number v-model.number="slotProps.editModel.postTaxRate" style="width:100%"></Input-number>
                    </FormItem>
                    <FormItem :label="$t('ProductClasses.BCTaxRate')" prop="bCTaxRate">
                        <Input-number v-model.number="slotProps.editModel.bCTaxRate" style="width:100%"></Input-number>
                    </FormItem>
                    <FormItem>
                        <Checkbox v-model="slotProps.editModel.isActive">{{$t('Public.IsActive')}}</Checkbox>
                    </FormItem>
                </TabPane>
            </Tabs>
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
    return {
      title: "ProductClasses",
      api: ProductClassApi,
      newProductClassRule: {
        className: [{ require: true }],
        pTId: [{ require: true }],
        postTaxRate: [{ type: "number" }],
        bCTaxRate: [{ type: "number" }]
      },
      productClassRule: {
        className: [{ require: true }],
        pTId: [{ require: true }],
        postTaxRate: [{ type: "number" }],
        bCTaxRate: [{ type: "number" }]
      },
      columnsetting: {
        needAction: true,
        columns: [
          {
            title: this.$t('ProductClasses.ClassName'),
            key: "className"
          },
          {
            title: this.$t('ProductClasses.PTId'),
            key: "pTId"
          },
          {
            title: this.$t('ProductClasses.PostTaxRate'),
            key: "postTaxRate"
          },
          {
            title: this.$t('ProductClasses.BCTaxRate'),
            key: "bCTaxRate"
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
      }
    };
  }
};
</script>