<template>
    <simplePage ref="simplepage" :title="title" 
        :columnsetting="columnsetting" 
        :api="api"
        :newRule="newSplitRuleRule"
        :editRule="splitRuleRule">
        <template slot="newform" slot-scope="slotProps">
            <Tabs value="detail">
                <TabPane :label="L('Details')" name="detail">
                    <FormItem :label="L('MinPackage')" prop="minPackage">
                        <Input v-model.number="slotProps.createModel.minPackage"></Input>
                    </FormItem>
                    <FormItem :label="L('MaxPackage')" prop="maxPackage">
                        <Input v-model.number="slotProps.createModel.maxPackage"></Input>
                    </FormItem>
                    <FormItem :label="L('MaxWeight')" prop="maxWeight">
                        <Input v-model.number="slotProps.createModel.maxWeight"></Input>
                    </FormItem>
                    <FormItem :label="L('MaxTax')" prop="maxTax">
                        <Input v-model.number="slotProps.createModel.maxTax"></Input>
                    </FormItem>
                    <FormItem :label="L('MaxPrice')" prop="maxPrice">
                        <Input v-model.number="slotProps.createModel.maxPrice"></Input>
                    </FormItem>
                    <FormItem :label="L('LogisticLineId')" prop="logisticLineId">
                        <Input v-model="slotProps.createModel.logisticLineId"></Input>
                    </FormItem>
                </TabPane>
            </Tabs>
        </template>
        <template slot="editform" slot-scope="slotProps">
            <Tabs value="detail">
                <TabPane :label="L('Details')" name="detail">
                    <FormItem :label="L('MinPackage')" prop="minPackage">
                        <Input v-model.number="slotProps.editModel.minPackage"></Input>
                    </FormItem>
                    <FormItem :label="L('MaxPackage')" prop="maxPackage">
                        <Input v-model.number="slotProps.editModel.maxPackage"></Input>
                    </FormItem>
                    <FormItem :label="L('MaxWeight')" prop="maxWeight">
                        <Input v-model.number="slotProps.editModel.maxWeight"></Input>
                    </FormItem>
                    <FormItem :label="L('MaxTax')" prop="maxTax">
                        <Input v-model.number="slotProps.editModel.maxTax"></Input>
                    </FormItem>
                    <FormItem :label="L('MaxPrice')" prop="maxPrice">
                        <Input v-model.number="slotProps.editModel.maxPrice"></Input>
                    </FormItem>
                    <FormItem :label="L('LogisticLineId')" prop="logisticLineId">
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
import SplitRuleApi from "@/api/splitRule";

export default {
  components: {
    simplePage
  },
  data() {
    return {
      title: "SplitRule",
      api: SplitRuleApi,
      newSplitRuleRule: {
        logisticLineId: [{ require: true }],
        minPackage: [{ require: true }],
        maxPackage: [{ require: true }],
        maxWeight: [{ require: true }],
        maxTax: [{ require: true }],
        maxPrice: [{ require: true }]
      },
      splitRuleRule: {
        logisticLineId: [{ require: true }],
        minPackage: [{ require: true }],
        maxPackage: [{ require: true }],
        maxWeight: [{ require: true }],
        maxTax: [{ require: true }],
        maxPrice: [{ require: true }]
      },
      columnsetting: {
        needAction: true,
        columns: [
          {
            title: this.L("MinPackage"),
            key: "minPackage"
          },
          {
            title: this.L("MaxPackage"),
            key: "maxPackage"
          },
          {
            title: this.L("MaxWeight"),
            key: "maxWeight"
          },
          {
            title: this.L("MaxTax"),
            key: "maxTax"
          },
          {
            title: this.L("MaxPrice"),
            key: "maxPrice"
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