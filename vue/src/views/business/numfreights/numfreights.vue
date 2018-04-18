<template>
    <simplePage ref="simplepage" :title="title" 
        :columnsetting="columnsetting" 
        :api="api"
        :newRule="newNumFreightRule"
        :editRule="numFreightRule">
        <template slot="newform" slot-scope="slotProps">
            <Tabs value="detail">
                <TabPane :label="L('Details')" name="detail">
                    <FormItem :label="L('PackagePrice')" prop="packagePrice">
                        <Input v-model.number="slotProps.createModel.packagePrice"></Input>
                    </FormItem>
                    <FormItem :label="L('ProductNum')" prop="productNum">
                        <Input v-model.number="slotProps.createModel.productNum"></Input>
                    </FormItem>
                    <FormItem :label="L('LogisticLine')" prop="logisticLineId">
                        <Input v-model="slotProps.createModel.logisticLineId"></Input>
                    </FormItem>
                </TabPane>
            </Tabs>
        </template>
        <template slot="editform" slot-scope="slotProps">
            <Tabs value="detail">
                <TabPane :label="L('Details')" name="detail">
                    <FormItem :label="L('PackagePrice')" prop="packagePrice">
                        <Input v-model.number="slotProps.editModel.packagePrice"></Input>
                    </FormItem>
                    <FormItem :label="L('ProductNum')" prop="productNum">
                        <Input v-model.number="slotProps.editModel.productNum"></Input>
                    </FormItem>
                    <FormItem :label="L('LogisticLine')" prop="logisticLineId">
                        <Input v-model="slotProps.editModel.logisticLineId"></Input>
                    </FormItem>
                    <FormItem>
                        <Checkbox v-model="slotProps.editModel.isActive">{{'IsActive'|l}}</Checkbox>
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
      title: "NumFreight",
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
            title: this.L("PackagePrice"),
            key: "packagePrice"
          },
          {
            title: this.L("ProductNum"),
            key: "productNum"
          },
          {
            title: this.L("LogisticLineId"),
            key: "logisticLineId"
          },
          {
            title: this.L("IsActive"),
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