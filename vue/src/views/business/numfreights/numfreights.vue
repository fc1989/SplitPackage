<template>
    <simplePage ref="simplepage" :title="title" 
        :columnsetting="columnsetting" 
        :api="api"
        :newRule="newNumFreightRule"
        :editRule="numFreightRule">
        <template slot="newform" slot-scope="slotProps">
            <Tabs value="detail">
                <TabPane :label="$t('Public.Details')" name="detail">
                    <FormItem :label="$t('NumFreights.PackagePrice')" prop="packagePrice">
                        <Input-number v-model.number="slotProps.createModel.packagePrice" style="width:100%"></Input-number>
                    </FormItem>
                    <FormItem :label="$t('NumFreights.ProductNum')" prop="productNum">
                        <Input-number v-model.number="slotProps.createModel.productNum" style="width:100%"></Input-number>
                    </FormItem>
                    <FormItem :label="$t('Menu.Pages.LogisticLines')" prop="logisticLineId">
                        <Input v-model="slotProps.createModel.logisticLineId"></Input>
                    </FormItem>
                </TabPane>
            </Tabs>
        </template>
        <template slot="editform" slot-scope="slotProps">
            <Tabs value="detail">
                <TabPane :label="$t('Public.Details')" name="detail">
                    <FormItem :label="$t('NumFreights.PackagePrice')" prop="packagePrice">
                        <Input-number v-model.number="slotProps.editModel.packagePrice" style="width:100%"></Input-number>
                    </FormItem>
                    <FormItem :label="$t('NumFreights.ProductNum')" prop="productNum">
                        <Input-number v-model.number="slotProps.editModel.productNum" style="width:100%"></Input-number>
                    </FormItem>
                    <FormItem :label="$t('Menu.Pages.LogisticLines')" prop="logisticLineId">
                        <Input v-model="slotProps.editModel.logisticLineId"></Input>
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
import NumFreightApi from "@/api/numFreight";

export default {
  components: {
    simplePage
  },
  data() {
    return {
      title: "Menu.Pages.NumFreights",
      api: NumFreightApi,
      newNumFreightRule: {
        packagePrice: [{ type: "number" }],
        productNum: [{ type: "number" }],
        logisticLineId: [{ require: true }]
      },
      numFreightRule: {
        packagePrice: [{ type: "number" }],
        productNum: [{ type: "number" }],
        logisticLineId: [{ require: true }]
      },
      columnsetting: {
        needAction: true,
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
            key: "logisticLineId"
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