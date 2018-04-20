<template>
    <simplePage ref="simplepage" :title="title" 
        :columnsetting="columnsetting" 
        :api="api"
        :newRule="newWeightFreightRule"
        :editRule="weightFreightRule">
        <template slot="newform" slot-scope="slotProps">
            <Tabs value="detail">
                <TabPane :label="$t('Public.Details')" name="detail">
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
                        <Input v-model="slotProps.createModel.logisticLineId"></Input>
                    </FormItem>
                </TabPane>
            </Tabs>
        </template>
        <template slot="editform" slot-scope="slotProps">
            <Tabs value="detail">
                <TabPane :label="$t('Public.Details')" name="detail">
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
import WeightFreightApi from "@/api/weightFreight";

export default {
  components: {
    simplePage
  },
  data() {
    return {
      title: "Menu.Pages.WeightFreights",
      api: WeightFreightApi,
      newWeightFreightRule: {
        startingWeight: [{ type: "number" }],
        startingPrice: [{ type: "number" }],
        stepWeight: [{ type: "number" }],
        price: [{ type: "number" }],
        logisticLineId: [{ type: "number" }]
      },
      weightFreightRule: {
        startingWeight: [{ type: "number" }],
        startingPrice: [{ type: "number" }],
        stepWeight: [{ type: "number" }],
        price: [{ type: "number" }],
        logisticLineId: [{ type: "number" }]
      },
      columnsetting: {
        needAction: true,
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