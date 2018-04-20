<template>
    <simplePage ref="simplepage" :title="title" 
        :columnsetting="columnsetting" 
        :api="api"
        :newRule="newSplitRuleRule"
        :editRule="splitRuleRule">
        <template slot="newform" slot-scope="slotProps">
            <Tabs value="detail">
                <TabPane :label="$t('Public.Details')" name="detail">
                    <FormItem :label="$t('SplitRules.MinPackage')" prop="minPackage">
                        <Input-number v-model.number="slotProps.createModel.minPackage" style="width:100%"></Input-number>
                    </FormItem>
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
                        <Input v-model="slotProps.createModel.logisticLineId"></Input>
                    </FormItem>
                </TabPane>
            </Tabs>
        </template>
        <template slot="editform" slot-scope="slotProps">
            <Tabs value="detail">
                <TabPane :label="$t('Public.Details')" name="detail">
                    <FormItem :label="$t('SplitRules.MinPackage')" prop="minPackage">
                        <Input-number v-model.number="slotProps.editModel.minPackage" style="width:100%"></Input-number>
                    </FormItem>
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
import SplitRuleApi from "@/api/splitRule";

export default {
  components: {
    simplePage
  },
  data() {
    return {
      title: "Menu.Pages.SplitRules",
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
            title: this.$t('SplitRules.MinPackage'),
            key: "minPackage"
          },
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